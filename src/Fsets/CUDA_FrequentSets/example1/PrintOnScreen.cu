void PrintTab()
{
	printf("\t");
}

void PrintNL()
{
	printf("\n");
}

void PrintTable(unsigned int** table, int rowsCount, int rowSize)
{
	int i,j;
	for(i=0; i < rowsCount; ++i)
	{
		for(j = 0; j < rowSize; ++j)
		{
			if(j % rowSize == 0 && i != 0)
				printf("\n");
			
				printf("%d ",table[i][j]);
		}
	}

	printf("\n\n");
}

void PrintSeparatorLine()
{
	printf("------------------------------------------------\n\n");
}
