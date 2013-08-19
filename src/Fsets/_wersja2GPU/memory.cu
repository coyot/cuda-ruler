#include "memory.h"

extern int *h_data;
extern int *d_data;
extern int *d_countOnes;
extern int *d_countTemp;
extern int *d_supportScan;
extern int *d_supportIndexes;
extern int *d_temp;
extern int *d_reducedData;
extern int *d_tempData;
extern int countTempSizeInBytes;
extern int countTempRowIntMultiple;
extern int **d_levelsData;
extern int **d_levelsIndexes;
extern int **h_levelsRowNumbers;
extern int **d_levelsRowNumbers;

void allocHostMemoryForFilter() {
	h_data = (int*) malloc(dataTableSizeInBytes);
	DEBUGONLY(printf("Host memory for filter allocated\n"));
}



void freeHostMemoryForFilter() {
	free(h_data);
	DEBUGONLY(printf("Host memory for filter freed\n");)
}



void allocHostMemoryForAlgo(int rows) {

	h_levelsRowNumbers=(int**)malloc(maxRecursion*sizeof(int*));
	
	for (int i=0;i<maxRecursion;i++) {
		h_levelsRowNumbers[i]=(int*)malloc(rows*sizeof(int));
	}
	
	DEBUGONLY(printf("Host memory for algo allocated\n");)
}

void freeHostMemoryForAlgo(int rows) {
	
	for (int i=0;i<maxRecursion;i++) {
		free(h_levelsRowNumbers[i]);		
	}

	free(h_levelsRowNumbers);

	DEBUGONLY(printf("Host memory for algo freed\n");)
}




