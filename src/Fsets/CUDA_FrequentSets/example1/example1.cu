#include "stdafx.h"

#include <stdio.h>
#include <cuda.h>

#include "Test.cu"
#include "Constants.cu"
#include "Init.cu"
#include "Errors.cu"
#include "PrintOnScreen.cu"
#include "GetSetsFromFile.cu"
#include "GetDefiniedItems.cu"
#include "BoolToIntConvert.cu"
#include "MakeBitmap.cu"
#include "GetRowSupremum.cu"
#include "GetFrequentSets.cu"
#include "GetRowTableBitAnd.cu"
#include "GetTableToBitAnd.cu"
#include "FindSets.cu"

#include "HostGetRowTableBitAnd.cu"
#include "HostFindSets.cu"

const int NUMBER_OF_ITERATIONS = 1;

unsigned int* gAndRow;
unsigned int** gFrequentSets;
unsigned int** gAndTable;
int* gFrequentSetsCount;
unsigned int* gIntVector;
bool* gBoolPacket;
bool** gInputBitmap;
unsigned int** gIntTable;
unsigned int** gFrequentBitmap;
int* gNewBitmapRowCount;
unsigned int** gNewBitmap;
unsigned int* gTable1D;
unsigned int** gAndTable2;

unsigned int testCount1;
unsigned int testCount2;

/*__device__*/ unsigned int *devRow1, *devRow2;
/*__device__*/ unsigned int* devGlobalAndTable;
/*__device__*/ unsigned int* devRow;

void StartCpuAndGpuFindSets()
{
	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);
	int* frequentSetsCount = (int*)malloc(sizeof(int*));
	unsigned int** frequentBitmap = GetFrequentSets(bitmap,
											NUMBER_OF_DIFFERENT_ITEMS,
											frequentSetsCount);

	clock_t startCPU, endCPU, startGPU, endGPU;
testCount1 = 0;
testCount2 = 0;

	startCPU = clock();
	__int64 startCpuTicks = GetTicks();

printf("this is start of cpu\n");
	//int i;
	//for(i = 0; i < NUMBER_OF_ITERATIONS; ++i)
	//{
		FindSets(frequentBitmap, *frequentSetsCount);
	//}
		printf("testCount: %d\n",testCount1);
printf("this is end of cpu\n");

	__int64 endCpuTicks = GetTicks();
	endCPU = clock();

	//printf("->End of CPU, start of GPU\n");

	startGPU = clock();
	__int64 startGpuTicks = GetTicks();
printf("this is start of GPU\n");
	//for(i = 0; i < NUMBER_OF_ITERATIONS; ++i)
	//{
		HostFindSets(frequentBitmap, *frequentSetsCount);
	//}
				printf("testCount: %d\n",testCount2);
printf("this is end of GPU\n");

	__int64 endGpuTicks = GetTicks();
	endGPU = clock();

	double cpuTime = (double)( endCPU - startCPU ) / (double)CLOCKS_PER_SEC;
	double gpuTime = (double)( endGPU - startGPU ) / (double)CLOCKS_PER_SEC;

	printf ("CPU processing time: %f [s]\n", cpuTime);
	printf ("GPU processing time: %f [s]\n", gpuTime);

	if(cpuTime < gpuTime)
	{
		double ratio = gpuTime / cpuTime;
		printf("\nCPU was faster %f times\n", ratio);
	}
	else
	{
		double ratio = cpuTime / gpuTime;
		printf("\nCUDA SUSSESS! GPU was faster %f times\n", ratio);
	}

	printf ("\nTicks summary:\n");
	printf ("Cpu ticks: ");
	PrintTicks(startCpuTicks, endCpuTicks);
	printf ("\nGpu ticks: ");
	PrintTicks(startGpuTicks, endGpuTicks);
}

//------------------------------ Start of HostGetRowTableBitAnd.cu file ---------------------------------//
// Kernel that executes on the CUDA device
__global__ void CudaGetBitAndOfRows(unsigned int* table1D, unsigned int* row, int rowSize, int tableRowCount)
{
	  int idx = blockIdx.x * blockDim.x + threadIdx.x;

	  if (idx < tableRowCount * rowSize)
	  {
		table1D[idx] = table1D[idx] & row[idx % rowSize];
	  }
}

void RunCudaGetBitAndOfRows(unsigned int* row, unsigned int* table1D, int tableRowCount)
{
	//__int64 start1 = GetTicks();
	//cudaMemcpy(devRow1, row1, NUMBER_OF_INTS_IN_ROW, cudaMemcpyHostToDevice);
	//cudaMemcpy(devRow2, row2, NUMBER_OF_INTS_IN_ROW, cudaMemcpyHostToDevice);
	cudaMemcpy(devGlobalAndTable,
			   table1D,
			   tableRowCount * NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int),
			   cudaMemcpyHostToDevice);

	cudaMemcpy(devRow, row, sizeof(unsigned int*), cudaMemcpyHostToDevice);
//__int64 stop1 = GetTicks();

	int blockSize = 32;
	int nBlocks = NUMBER_OF_INTS_IN_ROW / blockSize + (NUMBER_OF_INTS_IN_ROW % blockSize == 0 ? 0:1);
//__int64 start3 = GetTicks();
	CudaGetBitAndOfRows <<< nBlocks, blockSize >>> (devGlobalAndTable,
													devRow,
													NUMBER_OF_INTS_IN_ROW,
													tableRowCount);
//__int64 stop3 = GetTicks();
//__int64 start4 = GetTicks();
	cudaMemcpy(table1D,
			   devGlobalAndTable,
			   tableRowCount * NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int),
			   cudaMemcpyDeviceToHost);
//__int64 stop4 = GetTicks();

	/*printf ("\n\nRunCudaGetBitAndOfRoww total: ");
	PrintTicks(start1 , stop4);
	printf ("\ncudaMemcpy1: ");
	PrintTicks(start1 , stop1);
	printf ("\nCudaGetBitAndOfRows: ");
	PrintTicks(start3 , stop3);
	printf ("\ncudaMemcpy3: ");
	PrintTicks(start4 , stop4);*/
}

unsigned int** HostGetRowTableBitAnd(unsigned int* row, unsigned int** table, int tableRowCount)
{
	unsigned int* table1D =
		(unsigned int*)malloc(tableRowCount * NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));

	//__int64 start1 = GetTicks();

	int i,j;
	for(i = 0; i < tableRowCount; ++i)
	{
		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
		{
			table1D[i * NUMBER_OF_INTS_IN_ROW + j ] = table[i][j];
		}
	}
	//__int64 stop1 = GetTicks();

	//__int64 start2 = GetTicks();
	RunCudaGetBitAndOfRows(row, table1D, tableRowCount);
	//__int64 stop2 = GetTicks();

	unsigned int** andTable = (unsigned int**)malloc(tableRowCount * sizeof(unsigned int*));
	//__int64 start3 = GetTicks();
	for(i = 0; i < tableRowCount; ++i)
	{
		andTable[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(int));
		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
		{
			andTable[i][j] = table1D[i * NUMBER_OF_INTS_IN_ROW + j ];
		}
	}
	//__int64 stop3 = GetTicks();

/*	printf ("\n\nGpu statistics: ");
	printf ("Total time: ");
	PrintTicks(start1 , stop3);
	printf ("\nFirst loop: ");
	PrintTicks(start1 , stop1);
	printf ("\nBit and on gpu: ");
	PrintTicks(start2 , stop2);
	printf ("\nSecond loop: ");
	PrintTicks(start3 , stop3);*/
/*
	unsigned int** andTable = (unsigned int**)malloc(tableRowCount * sizeof(unsigned int*));
	int i,j;
	for(i = 0; i < tableRowCount; ++i)
	{
		andTable[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
		*andTable[i] = *row;
		RunCudaGetBitAndOfRows(andTable[i], table[i]);
	}
*/
	

	return andTable;
}

//------------------ Test ------------------//
void TestRunCudaGetBitAndOfRows()
{
	PrintTestStartMessage("RunCudaGetBitAndOfRows");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

//	RunCudaGetBitAndOfRows(bitmap[0], bitmap[1]);

	PrintTab();

	int i;
	for(i = 0; i < NUMBER_OF_INTS_IN_ROW; ++i)
		printf("%d ", bitmap[0][i]);

	PrintNL();

	PrintTestPassedMessage("RunCudaGetBitAndOfRows");
}

void TestHostGetRowTableBitAnd()
{
	PrintTestStartMessage("HostGetRowTableBitAnd");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

	unsigned int** bitAndTable = HostGetRowTableBitAnd(bitmap[0],
											   bitmap,
											   NUMBER_OF_DIFFERENT_ITEMS);
	int i,j;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		PrintTab();

		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
			printf("%d ", bitAndTable[i][j]);

		PrintNL();
	}

	PrintTestPassedMessage("HostGetRowTableBitAnd");
}

//------------------ End of Test ------------------//

//------------------------------ End of HostGetRowTableBitAnd.cu file ---------------------------------//

int main(void)
{
	InitVariables(); //This function must be run before other operations can begin
					 //Don't remove or comment this function!

	int i;

	gAndTable2 = (unsigned int**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(unsigned int*));
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		gAndTable2[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(int));

	gTable1D =
		(unsigned int*)malloc(NUMBER_OF_DIFFERENT_ITEMS * NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));

	gNewBitmap = (unsigned int**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(unsigned int*));
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		gNewBitmap[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));

	gNewBitmapRowCount = (int*)malloc(sizeof(int*));

/* Global pointers memory allocation */
    gAndRow = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
	gFrequentSets = (unsigned int**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(unsigned int*) );
	
	gAndTable = (unsigned int**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(unsigned int*));
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		gAndTable[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
	
	gFrequentSetsCount = (int*)malloc(sizeof(int*));
	gIntVector = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
	gBoolPacket = (bool*)malloc(NUMBER_OF_BITS_IN_INT * sizeof(bool));
	
	gInputBitmap = (bool**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(bool*));
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		gInputBitmap[i] = (bool*)malloc(NUMBER_OF_TRANSACTIONS * sizeof(bool));
	
	gIntTable = (unsigned int**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(unsigned int*));
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		gIntTable[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(int*));
	
	gFrequentBitmap = (unsigned int**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(unsigned int*));;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		gFrequentBitmap[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int*));

/* End of global pointers memory allocation */

	/*Don't remove or comment two lines belove. It is memeory allocation for GPU variables.*/
	//cudaMalloc((void**) &devRow1, NUMBER_OF_INTS_IN_ROW);
	//cudaMalloc((void**) &devRow2, NUMBER_OF_INTS_IN_ROW);

	cudaMalloc((void**) &devGlobalAndTable,
			   ((NUMBER_OF_DIFFERENT_ITEMS - 1) * NUMBER_OF_INTS_IN_ROW) * sizeof(unsigned int) );
	cudaMalloc((void**) &devRow, NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int) );

	//CPU testing
	//TestGetSetsFromFile();
	//TestMakeBitmapBool();
	//TestMakeIntVectorFromBitVector();
	/*TestMakeBitmap();
	TestGetTableToBitAnd();
	TestGetRowTableBitAnd();
	TestFindSets();*/

	//GPU testting
	//TestRunCudaGetBitAndOfRows();
	//TestHostFindSets();

	StartCpuAndGpuFindSets();

/* Free global pointers memory*/
    free(gAndRow);
	free(gFrequentSets);
	free(gAndTable);
	free(gFrequentSetsCount);
	free(gIntVector);
	free(gBoolPacket);
//	for(i = 0; i < NUMBER_OF_TRANSACTIONS; ++i)
//		free(gInputBitmap[i]);
	free(gInputBitmap);
	free(gIntTable);
/* End of free global pointers memory */

	//cudaFree(&devRow1);
	//cudaFree(&devRow2);
	cudaFree(&devGlobalAndTable);
	cudaFree(&devRow);

	getchar();
}
