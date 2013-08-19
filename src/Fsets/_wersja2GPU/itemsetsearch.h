#ifndef ITEMSETSEARCH_H
#define ITEMSETSEARCH_H

#include "utils.h"
#include "constants.h"
#include "memory.h"
#include "initialization.h"



int countTempSizeInBytes;
int countTempRowIntMultiple;
int *h_data;
int *d_data;
int *d_reducedData;
int *d_countOnes;
int *d_countTemp;
int *d_supportFlags;
int *d_supportScan;
int *d_supportIndexes;
int *d_temp;
int *d_tempData;
int **d_levelsData;
int **d_levelsIndexes;
int **h_levelsRowNumbers;
int **d_levelsRowNumbers;

__constant__ int dc_lookup[256];
int h_lookup[256];

int frequentItemset[maxRecursion];

struct freqItemsetListElem {
	int frequentItemset[maxRecursion];
	int length;
	struct 	freqItemsetListElem* next;
};

struct freqItemsetListElem* startOfList=NULL;

void addToList(int length) {
	struct freqItemsetListElem* oldStartOfList=startOfList;
	struct freqItemsetListElem* newElem=(struct freqItemsetListElem*)malloc(sizeof(struct freqItemsetListElem));
	memcpy(newElem->frequentItemset,frequentItemset,length*sizeof(int));
	newElem->length=length;
	newElem->next=oldStartOfList;
	startOfList=newElem;
	
}

void displayList() {
	int n=0;
	struct freqItemsetListElem* elem=startOfList;
	while (elem!=NULL) {
		for (int i=0;i<elem->length;i++) {
			printf("%d ",elem->frequentItemset[i]);
		}
		printf("\n");
		n++;
		elem=elem->next;
	}
	printf("%d frequent itemsets found \n",n);
}

__global__ void countOnesInInts( int *in, int* out, int n) 
{
	int i=blockDim.x*blockIdx.x+threadIdx.x;
	if (i<n) {
		int v=in[i];
		out[i]=dc_lookup[v&0xff]+dc_lookup[(v>>8)&0xff]+dc_lookup[(v>>16)&0xff]+dc_lookup[(v>>24)&0xff];
	}
}

//Ukradzione z reduction.pdf :D i trochę zmodyfikowane. Uwaga! optymalizacja możliwa. Użyłem tej wersji dla prostoty modyfikacji

__global__ void reduce(int *g_idata, int *g_odata,int n,int dist1, int dist2)
{
	extern __shared__ int sdata[];

	unsigned int i = blockIdx.x*blockDim.x + threadIdx.x;
		

	if (i<n) {
		unsigned int tid = threadIdx.x;
		unsigned int delta1=blockIdx.y*dist1;
		sdata[tid] = g_idata[i+delta1];
		__syncthreads();
		
		

		for (unsigned int s=blockDim.x/2; s>0; s>>=1) {
			if ((tid < s) && ((i+s)<n)) {
				sdata[tid] += sdata[tid + s];
			}
			__syncthreads();
			
		}

		if (threadIdx.x==0) g_odata[blockIdx.x+blockIdx.y*dist2]=sdata[0];

		
	}
}

__global__ void gather(int *in, int *out,int n,int step) {
	int i=blockIdx.x*blockDim.x + threadIdx.x;
	if (i<n) {
		out[i]=in[i*step];
	}
}

__global__ void checkValues(int *in, int* out, int treshold,int n) {
	int i=blockDim.x*blockIdx.x+threadIdx.x;
	if (i<n) {
		if (in[i]>=treshold) {
			out[i]=1;
		} else {
			out[i]=0;
		}
	}
	if (i==n) {
		out[i]=0;
	}
}

__global__ void scatter(int *in1, int treshold, int* in2, int* out,int n) {
	int i=blockDim.x*blockIdx.x+threadIdx.x;

	if (i<n) {
		if (in1[i]>=treshold) {
			out[in2[i]]=i;
		}
		/*if (i==n-1) {
			out[in2[i]+1]=-1;
		}*/
	}


}

template<int ignoreNumbers>
__global__ void gatherRows(int *in, int *inNumbers, int* indexes, int* out, int* outNumbers, int n) {
	
	int i=blockDim.x*blockIdx.x+threadIdx.x;
	int j=blockIdx.y;
	int k=indexes[j];

	if (i<n) {
		out[i+n*j]=in[i+n*k];
		if (ignoreNumbers!=1) {
			outNumbers[j]=inNumbers[k];
		}
	}
}

__global__ void rowAnd(int *inRow, int* inData, int* outData, int n) {
	int i=blockDim.x*blockIdx.x+threadIdx.x;
	int j=blockIdx.y;
	if (i<n) outData[i+n*j]=inData[i+n*j]&inRow[i];
}



//Oparte na publikacji nvr-2008-003.pdf
template <int kind>
__device__ int scan_warp ( volatile int * ptr , const unsigned int idx = threadIdx.x )
{
    const unsigned int lane = idx & 31; // index of thread in warp (0..31)
    
    if ( lane >= 1)  ptr [ idx ] =  ptr [ idx - 1]  + ptr [ idx ];
    if ( lane >= 2)  ptr [ idx ] =  ptr [ idx - 2]  + ptr [ idx ];
    if ( lane >= 4)  ptr [ idx ] =  ptr [ idx - 4]  + ptr [ idx ];
    if ( lane >= 8)  ptr [ idx ] =  ptr [ idx - 8]  + ptr [ idx ];
    if ( lane >= 16) ptr [ idx ] =  ptr [ idx - 16] + ptr [ idx ];
    

    if ( kind == 0 ) return ptr [ idx ]; //inclusive scan
    else return ( lane >0) ? ptr [ idx -1] : 0; //exclusive scan

}

//Oparte na publikacji nvr-2008-003.pdf
template <int kind>
__device__ int scan_block ( volatile int * ptr , const unsigned int idx = threadIdx.x )
{
    const unsigned int lane        = idx & 31;
    const unsigned int warpid = idx >> 5;
    // Step 1: Intra - warp scan in each warp
    int val=scan_warp<kind> ( ptr , idx );
    __syncthreads ();
    // Step 2: Collect per - warp partial results
    if ( lane ==31 ) ptr [ warpid ] = ptr [ idx ];
    __syncthreads ();
    // Step 3: Use 1 st warp to scan per - warp results
   if ( warpid ==0 ) scan_warp<kind> ( ptr , idx );

   __syncthreads ();
    // Step 4: Accumulate results from Steps 1 and 3
   if ( warpid > 0) val =  ptr [ warpid -1] + val ;
    __syncthreads ();
    // Step 5: Write and return the final result
    ptr [ idx ] = val ;
    __syncthreads ();
    return val ;
}

template <int kind>
__global__ void scan(int *data) {
	extern __shared__ int smem[];

	int i2=blockDim.x*blockIdx.x+threadIdx.x;
	
	int i1=threadIdx.x;
	smem[i1]=data[i2];
	scan_block<kind>(smem,threadIdx.x);
	data[i2]=smem[i1];
	
}

__global__ void gatherScan(int* data,int*temp,int prevBlockSize,int n) {
	if (threadIdx.x==blockDim.x-1) {
		temp[threadIdx.x]=data[n-1]; 
	} else {
		temp[threadIdx.x]=data[(threadIdx.x+1)*prevBlockSize-1];
	}
}

template <int kind>
__global__ void propagateScan(int* data,int *temp, int n) {
	extern __shared__ int smem[];
	int i=blockDim.x*blockIdx.x+threadIdx.x;
	
		if (kind==0) { //Inclusive scan
			data[i]+=temp[blockIdx.x];
		} else { //exclusive scan
			smem[(threadIdx.x+1)%blockDim.x]=data[i];	
			__syncthreads();
			if (i<n) {
				if (threadIdx.x==0) smem[0]=0;
				data[i]=smem[threadIdx.x]+=temp[blockIdx.x];
			}
		}

}



//Ograniczone do 512x512 elementów (2^18, czyli 262144) na potrzeby tej implementacji wystarczy
void scanCpuPart(int *data, int* temp, int n) {
	if (n>=512) {
		scan<0><<<n/512,512,512*sizeof(int)>>>(data);//Inclusive scan
	}
	if (n%512!=0) scan<0><<<1,n%512,n%512*sizeof(int)>>>(data+n-n%512); //Inclusive scan
	
	gatherScan<<<1,divRoundUp(n,512)>>>(data,temp,512,n);
	scan<1><<<1,divRoundUp(n,512),512*sizeof(int)>>>(temp); //Exclusive scan
	propagateScan<1><<<divRoundUp(n,512),512,512*sizeof(int)>>>(data,temp,n);
}

void generateAndCopyLookup() {
	for (int i=0; i<256 ; i++) {
		int r=0;
		for (int k=0;k<8;k++) {
			r+=((i&(1<<k))==0)?0:1;
		}
		h_lookup[i]=r;
	}
	DEBUGONLY(printf("Lookup generated \n");)

	cudaMemcpyToSymbol(dc_lookup, h_lookup, sizeof(h_lookup));
	DEBUGONLY(printf("Lookup copied to constant memory\n");)
}

//niszczy zarówno in jak i out
int sumTable(int* in, int *out, int n, int rows, int dist1, int dist2) {
	int *ping=in;
	int *pong=out;
	int *temp;	
	int dtemp;	

	int threads;
	int blocks=n;

	

	int iter=0;
	do {
		iter++;
		int oldBlocks=blocks;
		threads=f2plog2int(blocks);
		threads=threads>512?512:threads;
		blocks=blocks/threads;
		dim3 dimGrid(blocks,rows);
		reduce<<<dimGrid,threads,sizeof(int)*threads>>> (ping,pong,oldBlocks,dist1,dist2);
		temp=ping;
		ping=pong;
		pong=temp;
		dtemp=dist1;
		dist1=dist2;
		dist2=dtemp;
	} while (blocks>1);

	
	return iter;
}

//Zliczenie jedynek w pojedynczych intach
float countOnesInIntsCPU(int timer,int *in, int *out, int rows) {
	//Uruchom kernela zliczającego jedynki w każdym incie.
	float t1=cutGetTimerValue(timer);
	int blocks=rowSizeIntMultiple*rows;
	blocks=divRoundUp(blocks,512);//(blocks/512)+((blocks%512)!=0?1:0);
	countOnesInInts<<<blocks,512>>>(in,out,rowSizeIntMultiple*rows);
	float t2=cutGetTimerValue(timer);	

	return t2-t1;
}

//Zsumowanie jedynek w wierszach
float sumOnesInRowsCPU(int timer,int** sumResult,int rows) {
	float t1,t2;
	float total=0;

	int* ungatheredSumResult;
	
	t1=cutGetTimerValue(timer);
	int iter;

	iter=sumTable(d_countOnes,d_countTemp, rowSizeIntMultiple,rows,rowSizeIntMultiple,countTempRowIntMultiple) ;
	t2=cutGetTimerValue(timer);

	DEBUGONLY(printf("Sumowanie jedynek w wierszach %f[ms] \n",t2-t1);)
	total+=t2-t1;
	
	t1=cutGetTimerValue(timer);

	ungatheredSumResult=iter%2==1?d_countTemp:d_countOnes;
	*sumResult=iter%2==0?d_countTemp:d_countOnes;	

	gather<<<divRoundUp(rows,512),512>>>(ungatheredSumResult,*sumResult,rows,iter%2==1?countTempRowIntMultiple:rowSizeIntMultiple);
	t2=cutGetTimerValue(timer);

	//DEBUGONLY(displayDataTableGPU(*sumResult,1,rows,"%d ","Supports");)
	
	
	DEBUGONLY(printf("Zebranie wynikow dodawania do ciaglego obszaru pamieci %f[ms] \n",t2-t1);)

	total+=t2-t1;

	return total;
}


//Wybranie wierszy, których wsparcie>=minsup
float findFrequentItemsCPU(int timer, int* sumResult, int rows, int* out, int *resultingNumberOfRows) {
	float t1,t2;
	float total=0;
	
	t1=cutGetTimerValue(timer);
	checkValues<<<divRoundUp(rows+1,512),512>>>(sumResult,d_supportScan,minsup,rows); //Sztucznie dodaję jeden wiersz, aby wykonać dłuższy exclusive scan i uzyskać zawsze poprawną liczbę zbiorów częstych
	t2=cutGetTimerValue(timer);

	//DEBUGONLY(displayDataTableGPU(d_supportScan,1,rows+1,"%d ","CheckValues");)
	
	DEBUGONLY(printf("Sprawdzenie warunku wsparcia dla kazdego wiersza %f[ms] \n",t2-t1);)
	total+=t2-t1;

	t1=cutGetTimerValue(timer);
	scanCpuPart(d_supportScan,d_temp,rows+1);
	t2=cutGetTimerValue(timer);

	DEBUGONLY(printf("Znalezienie indeksow w scalonej tablicy (operacja scan) %f[ms] \n",t2-t1);)
	total+=t2-t1;

	t1=cutGetTimerValue(timer);
	scatter<<<divRoundUp(rows,512),512>>>(sumResult,minsup,d_supportScan,out,rows);	
	cutilSafeCall(cudaMemcpy(resultingNumberOfRows, d_supportScan+rows, sizeof(int),cudaMemcpyDeviceToHost) );
	
	//DEBUGONLY(displayDataTableGPU(out,1,*resultingNumberOfRows,"%d ","Indexes"));
	
	
	t2=cutGetTimerValue(timer);
	DEBUGONLY(printf("Znalezienie kolejnych numerow wierszy, ktore spelniaja warunek wsparcia %f[ms] \n",t2-t1);)
	total+=t2-t1;

	return total;
}


float filterTable(int timer,int *resultingNumberOfRows) {
	
	float t1,t2;
	float totaltime=0;
	
	
	//Skopiuj dane na kartę graficzną
	t1=cutGetTimerValue(timer);
	cutilSafeCall(cudaMemcpy(d_data, h_data, dataTableSizeInBytes,cudaMemcpyHostToDevice) );
	t2=cutGetTimerValue(timer);
	DEBUGONLY(printf("Kopiowanie danych na karte graficzna %f[ms]\n",t2-t1);)
	totaltime+=t2-t1;

	//Zlicz jedynki w intach
	t1=countOnesInIntsCPU(timer,d_data,d_countOnes,distinctItemCount);
	DEBUGONLY(printf("Zliczanie jedynek w intach %f[ms]\n",t1);)
	totaltime+=t1;
	
	//Zlicz jedynki w wierszach
	int *sumResult;
	t1=sumOnesInRowsCPU(timer,&sumResult,distinctItemCount);
	DEBUGONLY(printf("(TOTAL) Zliczanie jedynek w wierszach %f[ms] \n",t1);)
	totaltime+=t1;

	//Znajdź indeksy wierszy, które mają więcej jedynek niż minsup i umieść ich numery w kolejnych indeksach tablicy wynikowej
	
 	t1=findFrequentItemsCPU(timer,sumResult,distinctItemCount,d_supportIndexes,resultingNumberOfRows);
	DEBUGONLY(printf("(TOTAL) Frequent item search %f[ms] \n",t1);)
	DEBUGONLY(printf("Frequent items found %d\n",*resultingNumberOfRows);)


	cutilSafeCall(cudaMalloc((void**) &d_reducedData, *resultingNumberOfRows*rowSizeIntMultiple*sizeof(int))); 
	dim3 dimGrid(divRoundUp(rowSizeIntMultiple,512),*resultingNumberOfRows);
	gatherRows<1><<<dimGrid,512>>>(d_data, NULL,d_supportIndexes, d_reducedData, NULL,rowSizeIntMultiple); //Ignorujemy tablicę numerów wierszy
	

	totaltime+=t1;

	
	
	return totaltime;
}



void recMain(int timer,int level,int rows, int adjustment) {
	if (level<maxRecursion) {
		if (level==0) {			
			for (int i=0;i<rows-1;i++) {
				//printf(">>>>lvl %d row %d/%d adj %d\n",level,i,rows-1,adjustment);
				frequentItemset[level]=i;
				addToList(level+1);
				
				//Oblicz AND wiersza i ze wszystkimi wierszami reprezentującymi elementy częste.
				dim3 dimGrid(divRoundUp(rowSizeIntMultiple,512),rows-i-1);
				rowAnd<<<dimGrid,512>>>(d_data+i*rowSizeIntMultiple,d_data+(i+1)*rowSizeIntMultiple,d_tempData,rowSizeIntMultiple);
			
				//Policz jedynki w poszczególnych intach
				countOnesInIntsCPU(timer,d_tempData,d_countOnes,rows-i-1);

				//Zlicz jedynki w wierszach
				int *sumResult;
				sumOnesInRowsCPU(timer,&sumResult,rows-i-1);


				DEBUGONLY(displayDataTableGPU(sumResult,1,rows-i-1,"%d ","Supports");)


				//Znajdź zbiory częste
				int resultingNumberOfRows;
			 	findFrequentItemsCPU(timer,sumResult,rows-i-1,d_levelsIndexes[level],&resultingNumberOfRows);
				
				
				
				cutilSafeCall(cudaMemcpy(d_levelsRowNumbers[level],d_levelsIndexes[level],rows*sizeof(int),cudaMemcpyDeviceToDevice));
				
				dimGrid=dim3(divRoundUp(rowSizeIntMultiple,512),resultingNumberOfRows);
				gatherRows<1><<<dimGrid,512>>>(
						d_tempData, 
						NULL,
						d_levelsIndexes[level],
						d_levelsData[level],
						NULL,
						rowSizeIntMultiple);
				
				DEBUGONLY(displayDataTableGPU(d_levelsData[level],rowSizeIntMultiple,resultingNumberOfRows,"%x ","Partition");)

				recMain(timer,level+1,resultingNumberOfRows,i+1);
			}
			if (rows>=1) {
				DEBUGONLY(printf(">>>>lvl %d row %d/%d adj %d\n",level,rows-1,rows-1,adjustment);)
				frequentItemset[level]=rows-1;
				addToList(level+1);
			}
		} else {
			cutilSafeCall(cudaMemcpy(h_levelsRowNumbers[level-1], d_levelsRowNumbers[level-1], sizeof(int)*rows,cudaMemcpyDeviceToHost));
			for (int i=0;i<rows-1;i++) {
				//printf(">>>>lvl %d row %d/%d adj %d\n",level,i,rows-1,adjustment);
				frequentItemset[level]=h_levelsRowNumbers[level-1][i]+adjustment;
				addToList(level+1);
				//Oblicz AND wiersza i ze wszystkimi wierszami reprezentującymi elementy częste.
				
				dim3 dimGrid(divRoundUp(rowSizeIntMultiple,512),rows-i-1);
				rowAnd<<<dimGrid,512>>>(
					d_levelsData[level-1]+i*rowSizeIntMultiple, //Wskaźnik do początku wiersza z którym będziemy andować
					d_levelsData[level-1]+(i+1)*rowSizeIntMultiple, //Wskaźnik do początku wierszy z którymi będziemy andować
					d_tempData, //Gdzie zapisać wynik anda
					rowSizeIntMultiple); //Długość wiersza
			
				//Policz jedynki w poszczególnych intach
				countOnesInIntsCPU(timer,d_tempData,d_countOnes,rows-i-1);

				//Zlicz jedynki w wierszach
				int *sumResult;
				sumOnesInRowsCPU(timer,&sumResult,rows-i-1);

				DEBUGONLY(displayDataTableGPU(sumResult,1,rows-i-1,"%d ","Supports");)

				//Znajdź zbiory częste
				int resultingNumberOfRows;
			 	findFrequentItemsCPU(timer,sumResult,rows-i-1,d_levelsIndexes[level],&resultingNumberOfRows);
			
				dimGrid=dim3(divRoundUp(rowSizeIntMultiple,512),resultingNumberOfRows);						
				gatherRows<0><<<dimGrid,512>>>(d_tempData,d_levelsRowNumbers[level-1]+(i+1), d_levelsIndexes[level], d_levelsData[level], d_levelsRowNumbers[level],rowSizeIntMultiple);
				
				
				DEBUGONLY(displayDataTableGPU(d_levelsData[level],rowSizeIntMultiple,resultingNumberOfRows,"%x ","Partition");)

				
				recMain(timer,level+1,resultingNumberOfRows,adjustment);
			}
			if (rows>=1) {
				//printf(">>>>lvl %d row %d/%d adj %d\n",level,rows-1,rows-1,adjustment);
				frequentItemset[level]=h_levelsRowNumbers[level-1][rows-1]+adjustment;
				addToList(level+1);
			}
		}
	}
}

void runSearchHost() {
	DEBUGONLY(printf("Minimal support %d \n",minsup);)
	allocHostMemoryForFilter();
	allocDeviceMemoryForFilter();
	generateAndCopyLookup();
	generateData(h_data);
	


	DEBUGONLY(displayDataTableCPU(h_data,rowSizeIntMultiple, distinctItemCount,"%x ","Unfiltered starting data");)
	
	

	unsigned int timer = 0;
	cutilCheckError(cutCreateTimer(&timer));
	cutilCheckError(cutStartTimer(timer));

	int frequentItemsCount;

	filterTable(timer,&frequentItemsCount);
	
	DEBUGONLY(displayDataTableGPU(d_reducedData,rowSizeIntMultiple, frequentItemsCount,"%x ","Filtered data");)
	
	
	freeDeviceMemoryForFilter();
	freeHostMemoryForFilter();
	
	allocHostMemoryForAlgo(frequentItemsCount);
	allocDeviceMemoryForAlgo(frequentItemsCount);

	recMain(timer,0,frequentItemsCount,0);
	
	freeDeviceMemoryForAlgo(frequentItemsCount);
	freeHostMemoryForAlgo(frequentItemsCount);

	

	if (cudaThreadSynchronize()!=cudaSuccess) printf("BLEEEEE\n");
	printf("Processing time: %f (ms) \n", cutGetTimerValue(timer));

	DEBUGONLY(displayList();)

	//cutilSafeCall(cudaMemcpy(h_data, d_data, dataTableSizeInBytes,cudaMemcpyDeviceToHost) );

	
	
	/*for (int i=0;i<distinctItemCount;i++) {
		int supp=0;
		for (int k=0;k<rowSizeIntMultiple;k++) {	
			int bmp=h_data[i*rowSizeIntMultiple+k];
			supp+=h_lookup[bmp&0xff]+h_lookup[(bmp>>8)&0xff]+h_lookup[(bmp>>16)&0xff]+h_lookup[(bmp>>24)&0xff];
		}
		//printf("%d \n",supp);
	}*/

	cutilCheckError(cutStopTimer(timer));
	printf("Processing time: %f (ms) \n", cutGetTimerValue(timer));
	cutilCheckError(cutDeleteTimer(timer));

	
}

/*
void playWithScan() {
	int size=520;
	int *h_data = (int*) malloc(size*sizeof(int));
	int *d_data;
	cutilSafeCall(cudaMalloc((void**) &d_data, size*sizeof(int)));
	int *d_temp;
	

	for (int i=0;i<size;i++) {
		h_data[i]=1;//i%3==0?1:0;
	}

	cutilSafeCall(cudaMemcpy(d_data, h_data, size*sizeof(int),cudaMemcpyHostToDevice) );
	
	//scan<<<1,512,512*sizeof(int)>>>(d_data,5);

	unsigned int timer = 0;
	cutilCheckError(cutCreateTimer(&timer));
	cutilCheckError(cutStartTimer(timer));
	scanCpuPart(d_data,d_temp,size);

	
	cutilSafeCall(cudaMemcpy(h_data, d_data, size*sizeof(int),cudaMemcpyDeviceToHost) );
	cutilCheckError(cutStopTimer(timer));
	printf("Processing time: %f (ms) \n", cutGetTimerValue(timer));
	cutilCheckError(cutDeleteTimer(timer));

	for (int i=0;i<size;i++) {
		printf("%d ",h_data[i]);
	}
	printf("\n\n\n\n");

	cutilSafeCall(cudaMemcpy(h_data, d_temp, sizeof(int)*(size/512+((size%512)==0?0:1)),cudaMemcpyDeviceToHost) );

	for (int i=0;i<size;i++) {
		printf("%d ",h_data[i]);
	}
	printf("\n\n\n\n");

	cutilSafeCall(cudaFree(d_data));
	cutilSafeCall(cudaFree(d_temp));

}

*/
#endif


