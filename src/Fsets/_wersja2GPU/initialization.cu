#include "initialization.h"






void generateData(int* tab) {
	int items[frequentItemsetSize];
	memset(tab,0,dataTableSizeInBytes); //wyczysc tabele z danymi
	
	for (int i=0;i<frequentItemsetsToGenerate;i++) {
		for (int j=0;j<frequentItemsetSize;j++) {
			items[j]=(int)round(((distinctItemCount-1)*getDoubleRand()));
		}
		for (int j=0;j<1.1*minsup;j++) {
			int col=(int)round((itemsetCount-1)*getDoubleRand());
			for (int k=0;k<frequentItemsetSize;k++) {
				tab[items[k]*rowSizeIntMultiple+col/32]|=1<<col%32;
			}
		}
	}

}

//Zadziała poprawnie jedynie dla distinctItemCount=3 i itemsetCount=256. minsup=4
void generateData2(int* tab) {
	int rob[]={
		1,1,1,1,0,1,1,0,
		0,0,1,1,1,0,1,1,
		1,1,1,1,1,1,1,0};
	memcpy(tab,rob,24*4);
}

void generateData3(int* tab) {
	int rob[]={
		1,1,1,1,0,0,0,0,
		0,0,0,0,1,1,1,1,
		1,1,1,1,0,0,0,0,
		0,0,0,0,1,1,1,1,
		1,1,1,1,0,0,0,0};
	memcpy(tab,rob,5*8*sizeof(int));
}