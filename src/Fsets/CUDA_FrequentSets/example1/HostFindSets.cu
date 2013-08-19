unsigned int** HostGetRowTableBitAnd(unsigned int* row, unsigned int** table, int tableRowCount);
extern int* gNewBitmapRowCount;
extern unsigned int testCount2;

void HostFindSets(unsigned int** bitmap, int bitmapRowCount)
{
	//int* newBitmapRowCount = (int*)malloc(sizeof(int*));

	int i;
	for(i = 0; i < bitmapRowCount; ++i)
	{++testCount2;
		/* Uncoment line below for testing */
		//PrintNL(); 
		unsigned int** tableToBitAnd = GetTableToBitAnd(bitmap[i],
												bitmap,
												bitmapRowCount,
												gNewBitmapRowCount);

		if(*gNewBitmapRowCount > 0)
		{
			unsigned int** newBitmap = HostGetRowTableBitAnd(bitmap[i],
												 tableToBitAnd,
												 *gNewBitmapRowCount);

			int* frequentSetsCount = (int*)malloc(sizeof(int*)); 
			unsigned int** frequentBitmap =  GetFrequentSets(newBitmap,
													 *gNewBitmapRowCount,
													 frequentSetsCount);

			/*Uncoment block below this for testing */
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
				HostFindSets(frequentBitmap, *frequentSetsCount);
		}
	}
}


//------------------ Test ------------------//
void TestHostFindSets()
{
	PrintTestStartMessage("HostFindSets");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);
	int* frequentSetsCount = (int*)malloc(sizeof(int*));
	unsigned int** frequentBitmap = GetFrequentSets(bitmap,
											NUMBER_OF_DIFFERENT_ITEMS,
											frequentSetsCount);

	HostFindSets(frequentBitmap, *frequentSetsCount);

	PrintTestPassedMessage("HostFindSets");	
}

//------------------ End of Test ------------------//
