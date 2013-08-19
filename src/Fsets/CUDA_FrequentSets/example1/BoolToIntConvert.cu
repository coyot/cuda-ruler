#include <math.h>
#include <limits.h>

extern unsigned int* gIntVector;
extern bool* gBoolPacket;
extern unsigned int** gIntTable;

unsigned int** MakeBitmap(int** inputSets, int* definiedItems); //implemented in MakeBitmap.cu file
bool** MakeBitmapBool(int** inputSets, int* definiedItems); //implemented in MakeBitmap.cu file

/*
int GetIntMaxBit()
{
	int intMax = INT_MAX;

	int bitCount = 0;
	while(intMax != 1)
	{
		intMax /= 2;
		++bitCount;
	}

	printf("max int: %d\n", INT_MAX);
	printf("bit: %d\n", ++bitCount);
	return 0;
}*/

unsigned int MakeIntFromBool(bool* boolPacket, int rowsize)
{
	if(rowsize > NUMBER_OF_BITS_IN_INT)
		exit(-1); //error

	unsigned int intValue = 0;

	int i;
	int boolValue = 0;
	for(i = 0; i < rowsize; ++i)
	{
		(boolPacket[i] == true) ? boolValue = 1 : boolValue = 0;
		
		intValue += pow(2.0f,i) * boolValue;
	}

	return intValue;
}

unsigned int* MakeIntVectorFromBitVector(bool* bitmapRow)
{
	//unsigned int* intVector = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
	//bool* boolPacket = (bool*)malloc(NUMBER_OF_BITS_IN_INT * sizeof(bool));

	//boolPacket[0] = bitmapRow[0];

	int i, j;
	int boolPacketSize = NUMBER_OF_BITS_IN_INT;
	for(i = 0; i < NUMBER_OF_INTS_IN_ROW; ++i)
	{
		for(j = (i * NUMBER_OF_BITS_IN_INT); j < (i * NUMBER_OF_BITS_IN_INT + NUMBER_OF_BITS_IN_INT); ++j)
		{
			gBoolPacket[j - (i * NUMBER_OF_BITS_IN_INT)] = bitmapRow[j];
		}

		if( i == (NUMBER_OF_INTS_IN_ROW -1) )
		{
			boolPacketSize = NUMBER_OF_TRANSACTIONS % NUMBER_OF_BITS_IN_INT;

			//dla liczby transakcji <= 32
			if(boolPacketSize == 0 && NUMBER_OF_TRANSACTIONS > 0)
			{
				boolPacketSize = NUMBER_OF_BITS_IN_INT;
			}
		}

		gIntVector[i] = MakeIntFromBool(gBoolPacket, boolPacketSize);
	}

	return gIntVector;
}

unsigned int** MakeIntTableFromBitmap(bool** bitmap)
{
	int i;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		*gIntTable[i] = *(MakeIntVectorFromBitVector(bitmap[i]));
	}

	return gIntTable;
}

//------------------ Test ------------------//

void TestMakeIntFromBool()
{
	PrintTestStartMessage("MakeIntFromBool");

	bool* boolRow = (bool*)malloc(32*sizeof(bool));
	int i;
	for(i = 0; i < 32; ++i)
		boolRow[i] = true;

	int intValue = MakeIntFromBool(boolRow, 32);
	printf("intValue: %u\n", intValue);

	PrintTestPassedMessage("MakeIntFromBool");
}

void TestMakeIntVectorFromBitVector()
{
	PrintTestStartMessage("MakeIntVectorFromBitVector");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();

	bool** bitmap = MakeBitmapBool(setsFromFile, definiedItems);

	unsigned int* intVector = (unsigned int*)malloc(sizeof(unsigned int*));
	unsigned int* intVector2 = (unsigned int*)malloc(sizeof(unsigned int*));

	*intVector = *(MakeIntVectorFromBitVector(bitmap[0]));
	*intVector2 = *(MakeIntVectorFromBitVector(bitmap[1]));

	unsigned int test1= intVector[0];
	unsigned int test2= intVector2[0];

	int intCount = NUMBER_OF_TRANSACTIONS / NUMBER_OF_BITS_IN_INT;
	if( (NUMBER_OF_TRANSACTIONS % NUMBER_OF_BITS_IN_INT) != 0 )
		++intCount;

	int i;
	for(i = 0; i < intCount; ++i)
	{
		PrintTab();

		printf("%u ", intVector[i]);

		PrintNL();
	}

	for(i = 0; i < intCount; ++i)
	{
		PrintTab();

		printf("%u ", intVector2[i]);

		PrintNL();
	}

	intVector[0] = intVector[0] & intVector2[0];

	for(i = 0; i < intCount; ++i)
	{
		PrintTab();

		printf("%u ", intVector[i]);

		PrintNL();
	}

	PrintTestPassedMessage("MakeIntVectorFromBitVector");
}

void TestMakeIntTableFromBitmap()
{
	PrintTestStartMessage("MakeIntTableFromBitmap");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	bool** bitmap = MakeBitmapBool(setsFromFile, definiedItems);

	unsigned int** intTable = MakeIntTableFromBitmap(bitmap);

	int i, j;

	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		PrintTab();
		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
		{
			printf("%d ", bitmap[i][j]);
		}
		PrintNL();
	}

	PrintNL();

	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		PrintTab();

		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
		{
			printf("%u ", intTable[i][j]);
		}

		PrintNL();
	}

	PrintTestPassedMessage("MakeIntTableFromBitmap");
}

//------------------ End of Test ------------------//
