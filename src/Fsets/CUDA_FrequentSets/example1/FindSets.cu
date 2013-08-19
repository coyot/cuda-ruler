extern unsigned int** gFrequentBitmap;
extern unsigned int testCount1;

/*
Pseudo kod:
FindSets(sets)
{
	for(row = 0; row < rowCount; ++row)
	{
		tableToAnd = GetTableToAnd();

		newBitmap = GetRowTableBitAnd(row, tableToAnd);

		RemoveNotFrequentSets(newBitmap);

		if(newBitmapRowCount > 1)
			Rek(newBitmap)
	}
}
*/

void FindSets(unsigned int** bitmap, int bitmapRowCount)
{
	int* newBitmapRowCount = (int*)malloc(sizeof(int*));

	int i;
	for(i = 0; i < bitmapRowCount; ++i)
	{++testCount1;
		/* Uncoment line below for testing */
		//PrintNL();

		unsigned int** tableToBitAnd = GetTableToBitAnd(bitmap[i],
												bitmap,
												bitmapRowCount,
												newBitmapRowCount);
		if(*newBitmapRowCount > 0)
		{
			unsigned int** newBitmap = GetRowTableBitAnd(bitmap[i],
												 tableToBitAnd,
												 *newBitmapRowCount);

			int* frequentSetsCount = (int*)malloc(sizeof(int*)); 
			unsigned int** frequentBitmap =  GetFrequentSets(newBitmap,
													 *newBitmapRowCount,
													 frequentSetsCount);
			/* Uncoment block below this for testing */
			//
			//int x,y;
			//for(x = 0; x < *frequentSetsCount; ++x)
			//{
			//	PrintTab();

			//	for(y = 0; y < NUMBER_OF_INTS_IN_ROW; ++y)
			//		printf("%u ", frequentBitmap[x][y]);

			//	PrintNL();
			//}
			

			if(*frequentSetsCount > 1)
				FindSets(frequentBitmap, *frequentSetsCount);
		}
	}
}


//------------------ Test ------------------//
void TestFindSets()
{
	PrintTestStartMessage("FindSets");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);
	int* frequentSetsCount = (int*)malloc(sizeof(int*));
	unsigned int** frequentBitmap = GetFrequentSets(bitmap,
											NUMBER_OF_DIFFERENT_ITEMS,
											frequentSetsCount);

//	FindSets(frequentBitmap, *frequentSetsCount);

	PrintTestPassedMessage("FindSets");	
}

//------------------ End of Test ------------------//
