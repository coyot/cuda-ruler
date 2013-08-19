#ifndef UTILS_H
#define UTILS_H

#include <stdio.h>
//#include <cutil_inline.h>

const bool debug=false;

#define DEBUGONLY(x) if (debug) x


int f2plog2int(int x);
float getDoubleRand();
int divRoundUp(int x, int d);
void displayDataTableCPU(int* h_dataToDisplay,int cols,int rows,char* format,char* title);
void displayDataTableGPU(int* h_dataToDisplay,int cols,int rows,char* format,char* title);


#endif

