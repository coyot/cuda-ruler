unsigned int CountOnesInInt(unsigned int intValue)
{
	unsigned int onesCount = LOOK_UP_TABLE_BIT_COUNT[intValue & 0xff] + 
							 LOOK_UP_TABLE_BIT_COUNT[(intValue >> 8) & 0xff] + 
							 LOOK_UP_TABLE_BIT_COUNT[(intValue >> 16) & 0xff] + 
							 LOOK_UP_TABLE_BIT_COUNT[intValue >> 24]; 
	return onesCount;
}

int GetRowSupremum(unsigned int* row)
{
	unsigned int supremum = 0;
	
	int i;
	for(i = 0; i < NUMBER_OF_INTS_IN_ROW; ++i)
	{
		supremum += CountOnesInInt(row[i]);
	}

	return supremum;
}

//------------------ Test ------------------//

void TestGetRowSupremum()
{
	PrintTestStartMessage("GetRowSupremum");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

	int i,j;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i) 
	{
		int rowSuppremum = 0;
//		rowSuppremum = GetRowSupremum(bitmap[i]);
		
		PrintTab();
		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
			printf("%d ", bitmap[i][j]);
		
		printf("=> %d", rowSuppremum);

		PrintNL();
	}

	PrintTestPassedMessage("GetRowSupremum");
}

//------------------ End of Test ------------------//