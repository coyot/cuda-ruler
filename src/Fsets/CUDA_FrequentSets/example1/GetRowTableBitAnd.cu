extern unsigned int* gAndRow;
extern unsigned int** gAndTable;

unsigned int* GetBitAndOfRows(unsigned int* row1, unsigned int* row2)
{
	//unsigned int* andRow = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
	
	int i;

	//*gAndRow = *row1 & *row2;		

	for(i = 0; i < NUMBER_OF_INTS_IN_ROW; ++i)
	{
		gAndRow[i] = row1[i] & row2[i];
	}
	
	return gAndRow;
}

unsigned int** GetRowTableBitAnd(unsigned int* row, unsigned int** table, int tableRowCount)
{
	unsigned int** andTable = (unsigned int**)malloc(tableRowCount * sizeof(unsigned int*));
	int i;

	//__int64 start1 = GetTicks();
	for(i = 0; i < tableRowCount; ++i)
	{
		andTable[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));
		*andTable[i] = *GetBitAndOfRows(row, table[i]);
	}
	//__int64 stop1 = GetTicks();

	//printf ("\nBit and on cpu: ");
	//PrintTicks(start1 , stop1);

	return andTable;
}

//------------------ Test ------------------//
void TestGetBitAndOfRows()
{
	PrintTestStartMessage("GetRowTableBitAnd");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

//	unsigned int* bitAnd = GetBitAndOfRows(bitmap[0], bitmap[1]);

	PrintTab();

	int i;
	for(i = 0; i < NUMBER_OF_INTS_IN_ROW; ++i)
//		printf("%d ", bitAnd[i]);

	PrintNL();

	PrintTestPassedMessage("GetRowTableBitAnd");
}

void TestGetRowTableBitAnd()
{
	PrintTestStartMessage("GetRowTableBitAnd");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

	//unsigned int** bitAndTable = GetRowTableBitAnd(bitmap[0],
	//									   bitmap,
	//									   NUMBER_OF_DIFFERENT_ITEMS);
	int i,j;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		PrintTab();

		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
//			printf("%u ", bitAndTable[i][j]);

		PrintNL();
	}

	PrintTestPassedMessage("GetRowTableBitAnd");
}

//------------------ End of Test ------------------//