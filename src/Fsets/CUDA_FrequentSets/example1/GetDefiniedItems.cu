int* GetDefinedItems()
{
	int* definedItems = (int*)malloc(NUMBER_OF_DIFFERENT_ITEMS * sizeof(int));

	for(int i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		definedItems[i] = i+1;

	return definedItems;
}

//------------------ Test ------------------//

void TestGetDefinedItems()
{
	PrintTestStartMessage("GetDefinedItems");
	PrintTab();

	int* definiedItems = GetDefinedItems();

	int i;
	for(i = 0; i < NUMBER_OF_DIFFERENT_ITEMS; ++i)
		printf("%d ", definiedItems[i]);

	PrintNL();

	PrintTestPassedMessage("GetDefinedItems");
}

//------------------ End of Test ------------------//