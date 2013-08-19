unsigned int** GetTableToBitAnd(unsigned int* row,
						 unsigned int** bitmap,
						 int bitmapRowCount,
						 int* newBitmapRowCount)
{
	int rowIndex;
	for(rowIndex = 0; rowIndex < bitmapRowCount; ++rowIndex)
	{
		if(bitmap[rowIndex] == row)
		{
			break;
		}
	}

	*newBitmapRowCount = bitmapRowCount - rowIndex - 1;
	unsigned int** newBitmap = (unsigned int**)malloc(*newBitmapRowCount * sizeof(unsigned int*));
	int i;
	for(i = 0; i < *newBitmapRowCount; ++i)
		newBitmap[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(unsigned int));

	int nextRow = rowIndex + 1;
	for(i = nextRow; i < bitmapRowCount; ++i)
	{
		newBitmap[i - nextRow] = bitmap[i];
	}

	return newBitmap;
}

//------------------ Test ------------------//
void TestGetTableToBitAnd()
{
	PrintTestStartMessage("GetTableToBitAnd");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

	int* newBitmapRowCount = (int*)malloc(sizeof(int));
	unsigned int** tableToAnd = GetTableToBitAnd(bitmap[0],
										 bitmap,
										 NUMBER_OF_DIFFERENT_ITEMS,
										 newBitmapRowCount);
	
	int i,j;
	for(i = 0; i < *newBitmapRowCount ; ++i)
	{
		PrintTab();

		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
			printf("%d ", tableToAnd[i][j]);

		PrintNL();
	}

	PrintTestPassedMessage("GetTableToBitAnd");
}

//------------------ End of Test ------------------//
