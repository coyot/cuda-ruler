#include "utils.h"

int f2plog2int(int x) {
	x |= x>>1;
	x |= x>>2;
	x |= x>>4;
	x |= x>>8;
	x |= x>>16;
	return x - (x>>1);
}

float getDoubleRand() {
	return (double)rand()/(double)RAND_MAX;
}

int divRoundUp(int x,int d) {
	return x/d+((x%d)==0?0:1);
}

void displayDataTableCPU(int* h_dataToDisplay,int cols,int rows,char* format,char* title) {
	printf("\n>>>>%s<<<\n",title);
	for (int i=0;i<rows;i++) {
		for (int j=0;j<cols;j++) {
			printf(format,h_dataToDisplay[cols*i+j]);
}
		printf("\n");
}
	printf("\n");
}

