extern unsigned int** gFrequentSets;
extern int* gFrequentSetsCount;

unsigned int** GetFrequentSets(unsigned int** inputSets,
							   int inputSetsCount,
							   int* frequentSetsCount)
{
	unsigned int** frequentSets = (unsigned int**)malloc(inputSetsCount * sizeof(unsigned int*) );
	int i;
	for(i = 0; i < inputSetsCount; ++i)
		frequentSets[i] = (unsigned int*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(int));

	int frequentSetIndex = 0;

	for(i = 0; i < inputSetsCount; ++i)
	{
		if(GetRowSupremum(inputSets[i]) >= MIN_SUP)
		{
			frequentSets[frequentSetIndex] = inputSets[i];
			++frequentSetIndex;
		}
	}

	*frequentSetsCount = frequentSetIndex;
	return frequentSets;
}

//------------------ Test ------------------//

void TestGetFrequentSets()
{
	PrintTestStartMessage("GetFrequentSets");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();
	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

	//int* frequentSetsCount = (int*)malloc(sizeof(int));
//	unsigned int** frequentSets = GetFrequentSets(bitmap,
//										  NUMBER_OF_DIFFERENT_ITEMS,
//										  frequentSetsCount);
	int i,j;
	for(i = 0; i < *gFrequentSetsCount; ++i)
	{
		PrintTab();

		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
//			printf("%d ", frequentSets[i][j]);
		
		PrintNL();
	}

	PrintTestPassedMessage("GetFrequentSets");
}

//------------------ End of Test ------------------//
