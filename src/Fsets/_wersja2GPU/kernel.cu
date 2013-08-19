#include <math.h>
#include <stdio.h>
#include <cutil_inline.h>
#include "cudpp.h"
#include "itemsetsearch.h"

void displayDataTableGPU(int* d_dataToDisplay,int cols,int rows,char* format,char* title) {
	int* buffer=(int*)malloc(cols*rows*sizeof(int));
	cutilSafeCall(cudaMemcpy(buffer,d_dataToDisplay,cols*rows*sizeof(int),cudaMemcpyDeviceToHost));
	displayDataTableCPU(buffer,cols,rows,format,title);
	free(buffer);
}

void allocDeviceMemoryForFilter() {
	cutilSafeCall(cudaMalloc((void**) &d_data, dataTableSizeInBytes));
	cutilSafeCall(cudaMalloc((void**) &d_countOnes, dataTableSizeInBytes));

	int threads=f2plog2int(rowSizeIntMultiple);
	threads=threads>512?512:threads;
	int blocks=rowSizeIntMultiple/threads;


	countTempRowIntMultiple=blocks; 
	countTempSizeInBytes=countTempRowIntMultiple*sizeof(int)*distinctItemCount;

	cutilSafeCall(cudaMalloc((void**) &d_countTemp, countTempSizeInBytes)); 
	cutilSafeCall(cudaMalloc((void**) &d_supportScan,sizeof(int)*(distinctItemCount+1)));
	cutilSafeCall(cudaMalloc((void**) &d_supportIndexes,sizeof(int)*distinctItemCount));
	cutilSafeCall(cudaMalloc((void**) &d_temp, 512*sizeof(int)));
	cutilSafeCall(cudaMalloc((void**) &d_reducedData, dataTableSizeInBytes));
	
	
	DEBUGONLY(printf("Device memory for filter allocated\n");)
}

void freeDeviceMemoryForFilter() {
	//cutilSafeCall(cudaFree(d_supportScan));
	//cutilSafeCall(cudaFree(d_supportIndexes));
	//cutilSafeCall(cudaFree(d_temp));
	//cutilSafeCall(cudaFree(d_rowNumbers));
	cutilSafeCall(cudaFree(d_countOnes));
	cutilSafeCall(cudaFree(d_countTemp));
	cutilSafeCall(cudaFree(d_data));
	DEBUGONLY(printf("Device memory for filter freed\n");)
}

void allocDeviceMemoryForAlgo(int rows) {
	//cutilSafeCall(cudaMalloc((void**) &d_data, rows*rowSizeIntMultiple*sizeof(int)));
	d_data=d_reducedData;
	
	cutilSafeCall(cudaMalloc((void**) &d_tempData, rows*rowSizeIntMultiple*sizeof(int)));
	cutilSafeCall(cudaMalloc((void**) &d_countOnes,  rows*rowSizeIntMultiple*sizeof(int)));


	int threads=f2plog2int(rowSizeIntMultiple);
	threads=threads>512?512:threads;
	int blocks=rowSizeIntMultiple/threads;


	countTempRowIntMultiple=blocks; 
	countTempSizeInBytes=countTempRowIntMultiple*sizeof(int)*rows;

	cutilSafeCall(cudaMalloc((void**) &d_countTemp, countTempSizeInBytes)); 


	d_levelsData=(int**)malloc(maxRecursion*sizeof(int*));
	d_levelsIndexes=(int**)malloc(maxRecursion*sizeof(int*));
	d_levelsRowNumbers=(int**)malloc(maxRecursion*sizeof(int*));
	

	for (int i=0;i<maxRecursion;i++) {
		cutilSafeCall(cudaMalloc((void**) &d_levelsData[i],rows*rowSizeIntMultiple*sizeof(int)));
		cutilSafeCall(cudaMalloc((void**) &d_levelsIndexes[i],rows*sizeof(int)));
		cutilSafeCall(cudaMalloc((void**) &d_levelsRowNumbers[i], distinctItemCount*sizeof(int)));
	}

	DEBUGONLY(printf("Device memory for algo allocated\n");)
}

void freeDeviceMemoryForAlgo(int rows) {
	cutilSafeCall(cudaFree( d_reducedData));
	cutilSafeCall(cudaFree( d_tempData));
	cutilSafeCall(cudaFree(d_supportIndexes));
	
	for (int i=0;i<maxRecursion;i++) {	
		cutilSafeCall(cudaFree(d_levelsData[i]));
		cutilSafeCall(cudaFree(d_levelsIndexes[i]));
		cutilSafeCall(cudaFree(d_levelsRowNumbers[i]));
	}
	free(d_levelsData);
	free(d_levelsIndexes);
	free(d_levelsRowNumbers);
	
	DEBUGONLY(printf("Device memory for algo freed\n");)
}

int main(int argc, char** argv) {
	//printf("%d\n",RAND_MAX);
	if( cutCheckCmdLineFlag(argc, (const char**)argv, "device") )
		cutilDeviceInit(argc, argv);
	else
		cudaSetDevice( cutGetMaxGflopsDeviceId() );

	//runBinarySingleBlock();
	//runHostBinary();
	//runBinarySingleBlock();
	/*runSearchHost();
	runSearchDevice();
	runSearchDevice2();*/
	runSearchHost();
	//playWithScan();
	//int a;
	//scanf("%d",&a);
	cudaThreadExit();
	getchar();
}

