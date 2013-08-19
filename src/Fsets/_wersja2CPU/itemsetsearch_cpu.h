#ifndef ITEMSETSEARCH_CPU_H
#define ITEMSETSEARCH_CPU_H
//includować tylko ten plik albo itemsetsearch

#include "constants.h"
#include "utils.h"
#include "initialization.h"
#include <stdlib.h>
#include <string.h>
#include <cutil_inline.h>
using namespace std;

int h_lookup[256];

int frequentItemset[maxRecursion];

int h_supports[distinctItemCount];

int **h_levelsData;//[maxRecursion][distinctItemCount];
int h_reducedData[rowSizeIntMultiple*distinctItemCount];
int h_data[rowSizeIntMultiple*distinctItemCount];
int h_tempAnd[rowSizeIntMultiple*distinctItemCount];
int h_countOnes[rowSizeIntMultiple*distinctItemCount];

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

void generateLookup() {
	for (int i=0; i<256 ; i++) {
		int r=0;
		for (int k=0;k<8;k++) {
			r+=((i&(1<<k))==0)?0:1;
		}
		h_lookup[i]=r;
	}
	DEBUGONLY(printf("Lookup generated \n");)
}


float filterTable(int timer,int *in, int *out,int rows,int *resultingNumberOfRows) {
	
	float t1,t2;
	float totaltime=0;
	
	
	for (int i=0;i<rows;i++) {
		int supp=0;
		for (int k=0;k<rowSizeIntMultiple;k++) {	
			int bmp=in[i*rowSizeIntMultiple+k];
			supp+=h_lookup[bmp&0xff]+h_lookup[(bmp>>8)&0xff]+h_lookup[(bmp>>16)&0xff]+h_lookup[(bmp>>24)&0xff];
		}
		h_supports[i]=supp;
	}
	
	DEBUGONLY(displayDataTableCPU(h_supports,1,rows,"%d ","Supports");)


	int index=0;
	for (int i=0;i<rows;i++) {
		if (h_supports[i]>=minsup) {
			for (int k=0;k<rowSizeIntMultiple;k++) {
				out[index*rowSizeIntMultiple+k]=in[i*rowSizeIntMultiple+k];
			}
			index++;
		}
	}
	
	*resultingNumberOfRows=index;
	
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
				for (int j=i+1;j<rows;j++) {
					for (int k=0;k<rowSizeIntMultiple;k++) {
						h_tempAnd[(j-i-1)*rowSizeIntMultiple+k]=h_reducedData[j*rowSizeIntMultiple+k]&h_reducedData[i*rowSizeIntMultiple+k];
					}
				}

				int resultingNumberOfRows;
				filterTable(timer,h_tempAnd,h_levelsData[level],rows-i-1,&resultingNumberOfRows);
			
				DEBUGONLY(displayDataTableCPU(h_levelsData[level],rowSizeIntMultiple,resultingNumberOfRows,"%x ","Partition");)
				recMain(timer,level+1,resultingNumberOfRows,i+1);
			}
			if (rows>=1) {
				//printf(">>>>lvl %d row %d/%d adj %d\n",level,rows-1,rows-1,adjustment);
				frequentItemset[level]=rows-1;
				addToList(level+1);
			}
		} else {
			for (int i=0;i<rows-1;i++) {
				//printf(">>>>lvl %d row %d/%d adj %d\n",level,i,rows-1,adjustment);
				frequentItemset[level]=i;//h_levelsRowNumbers[level-1][i]+adjustment;
				addToList(level+1);
				//Oblicz AND wiersza i ze wszystkimi wierszami reprezentującymi elementy częste.
				
				
				for (int j=i+1;j<rows;j++) {
					for (int k=0;k<rowSizeIntMultiple;k++) {
						h_tempAnd[(j-i-1)*rowSizeIntMultiple+k]=(h_levelsData[level-1])[j*rowSizeIntMultiple+k]&(h_levelsData[level-1])[i*rowSizeIntMultiple+k];
						
					}
					
					
				}

				int resultingNumberOfRows;
				filterTable(timer,h_tempAnd,h_levelsData[level],rows-i-1,&resultingNumberOfRows);

				DEBUGONLY(displayDataTableCPU(h_levelsData[level],rowSizeIntMultiple,resultingNumberOfRows,"%x ","Partition");)

				recMain(timer,level+1,resultingNumberOfRows,adjustment);
			}
			if (rows>=1) {
				//printf(">>>>lvl %d row %d/%d adj %d\n",level,rows-1,rows-1,adjustment);
				frequentItemset[level]=rows-1;//h_levelsRowNumbers[level-1][rows-1]+adjustment;
				addToList(level+1);
			}
		}
	}
}

void runSearchHost() {
	DEBUGONLY(printf("Minimal support %d \n",minsup);)
	generateLookup();
	generateData(h_data);

	DEBUGONLY(displayDataTableCPU(h_data,rowSizeIntMultiple, distinctItemCount,"%x ","Unfiltered starting data");)

	unsigned int timer = 0;
	cutilCheckError(cutCreateTimer(&timer));
	cutilCheckError(cutStartTimer(timer));
	int resultingNumberOfRows;
	h_levelsData=(int**)malloc(maxRecursion*sizeof(int*));

	for (int i=0;i<maxRecursion;i++) {
		h_levelsData[i]=(int*)malloc(distinctItemCount*rowSizeIntMultiple*sizeof(int));
	}

	filterTable(timer,h_data, h_reducedData,distinctItemCount,&resultingNumberOfRows);
	//recMain(timer,0,resultingNumberOfRows, 0);

	cutilCheckError(cutStopTimer(timer));
	printf("Processing time: %f (ms) \n", cutGetTimerValue(timer));
	cutilCheckError(cutDeleteTimer(timer));


	for (int i=0;i<maxRecursion;i++) {
		free(h_levelsData[i]);
	}
	free(h_levelsData);

	DEBUGONLY(displayList();)
	//free(h_data);
}

#endif