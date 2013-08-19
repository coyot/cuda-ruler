#ifndef MEMORY_H
#define MEMORY_H

#include "utils.h"
#include <stdio.h>
//#include <cutil_inline.h>
#include "constants.h"


void allocHostMemoryForFilter();
void allocDeviceMemoryForFilter();
void freeHostMemoryForFilter();
void freeDeviceMemoryForFilter();

void allocHostMemoryForAlgo(int rows);
void allocDeviceMemoryForAlgo(int rows);
void freeHostMemoryForAlgo(int rows);
void freeDeviceMemoryForAlgo(int rows);





#endif

