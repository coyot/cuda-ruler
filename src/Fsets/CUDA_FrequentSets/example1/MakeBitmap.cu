extern bool** gInputBitmap;
extern unsigned int** gIntTable;

unsigned int ItemIsInTransation(int item, int** transactions, int transationIndex)
{
	unsigned int itemIsInTransation = false;
	for(int i = 0; i < MAX_TRANSACTION_SIZE; ++i)
	{
		if(transactions[transationIndex][i] == item)
		{
			itemIsInTransation = true;
			break;
		}
	}
	
	return itemIsInTransation;
}

bool** MakeBitmapBool(int** inputSets, int* definiedItems)
{
	//Bitmapa to tablica o wymiarach: liczbaZdefiniowanychElementow * LiczbaTransakcji
	//Wierszom bitmapy odpowiadaja kolejne elementy ze zbioru zdefiniowanych elementow
	//Kolumny bitmapy odpowiadaja klejnym transakcjom
	//np. dla danych: 1. chleb, maka, jajka 2. jajka 3. chleb maka
	//zbior elementow to: chleb, maka, jajka, transakcje: 1, 2, 3
	//Bitmapa:    
    //				 1  2  3
	//        chleb  1  0  1
	//		  jajka  1  1  0
	//		  maka   1  0  1

	//bool** inputBitmap = (bool**)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(bool*));
	int i;
	//for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	//	inputBitmap[i] = (bool*)malloc(NUMBER_OF_INTS_IN_ROW * sizeof(bool));

	for(int itemIndex = 0; itemIndex < NUMBER_OF_DIFFERENT_ITEMS; ++itemIndex)
	{
		int transactionIndex;
		for(transactionIndex = 0; transactionIndex < NUMBER_OF_TRANSACTIONS; ++transactionIndex)
		{
			if( true == ItemIsInTransation(definiedItems[itemIndex], inputSets, transactionIndex) )
				gInputBitmap[itemIndex][transactionIndex] = true;
			else
				gInputBitmap[itemIndex][transactionIndex] = false;
		}
	}
	return gInputBitmap;
}

unsigned int** MakeBitmap(int** inputSets, int* definiedItems)
{
	bool** boolBitmap = MakeBitmapBool(inputSets, definiedItems);

	unsigned int** intBitmap = MakeIntTableFromBitmap(boolBitmap);

	return intBitmap;
}


//------------------ Test ------------------//
void TestMakeBitmapBool()
{
	PrintTestStartMessage("MakeBitmapBool");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();	

	bool** bitmap = MakeBitmapBool(setsFromFile, definiedItems);

	int i,j;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		PrintTab();
		for(j = 0; j < NUMBER_OF_TRANSACTIONS; ++j)
		{
			printf("%d ", bitmap[i][j]);
		}
		PrintNL();
	}

	PrintTestPassedMessage("MakeBitmapBool");
}

void TestMakeBitmap()
{
	PrintTestStartMessage("MakeBitmap");

	int** setsFromFile = GetSetsFromFile();
	int* definiedItems = GetDefinedItems();

	unsigned int** bitmap = MakeBitmap(setsFromFile, definiedItems);

	int i,j;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
	{
		PrintTab();
		for(j = 0; j < NUMBER_OF_INTS_IN_ROW; ++j)
		{
			printf("%d ", bitmap[i][j]);
		}
		PrintNL();
	}

	PrintTestPassedMessage("MakeBitmap");
}

//------------------ End of Test ------------------//
