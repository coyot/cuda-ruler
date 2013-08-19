//------------------ String operations ------------------//
int GetCharIndex(char* text, int textSize, char charToFind)
{
	int charIndex = -1;

	int i = 0;
	while(text[i] != '\0')
	{
		if(text[i] == charToFind)
		{
			charIndex = i;
			break;
		}

		++i;
	}
	return charIndex;
}

char* MakeSubstringFromStart(char* oryginalString, int length, char* substring)
{
	if(length < 0)
	{
		substring = oryginalString;
	}
	else
	{
		int newLength = length + 1;

		int i;
		for(i = 0; i < length; ++i)
			substring[i] = oryginalString[i];

		substring[newLength-1] = '\0';
	}

	return substring;
}

char* MakeSubstringToEnd(char* oryginalString, int startIndex, char* substring)
{
	int newLength = strlen(oryginalString) - startIndex + 1;
	int i;
	for(i = startIndex; i < strlen(oryginalString); ++i)
		substring[i - startIndex] = oryginalString[i];

	substring[newLength-1] = '\0';

	return substring;
}

//------------------ End of String operations ------------------//

int** GetSetsFromFile()
{
	int** sets = (int**)malloc(NUMBER_OF_TRANSACTIONS * sizeof(int*));
	int i;
	for(i = 0; i < NUMBER_OF_TRANSACTIONS; ++i)
		sets[i] = (int*)malloc(MAX_TRANSACTION_SIZE * sizeof(int));
	
	for(i = 0; i < NUMBER_OF_TRANSACTIONS; ++i)
	{
		int j;
		for(j = 0; j < MAX_TRANSACTION_SIZE; ++j)
		{
			sets[i][j] = 0;
		}
	}

	FILE *file = fopen ( "Dane.txt", "r" );

	if ( file != NULL )
	{
		char* line = (char*)malloc(MAX_FILE_LINE_SIZE * sizeof(char));
		char* substring1 = (char*)malloc(MAX_FILE_LINE_SIZE * sizeof(char));
		char* substring2 = (char*)malloc(MAX_FILE_LINE_SIZE * sizeof(char));
		char* number;

		int i;
		for(i = 0; i < NUMBER_OF_TRANSACTIONS; ++i )
		{
			fgets ( line, MAX_FILE_LINE_SIZE, file );
				
			int separatorIndex = 1;
			int j;
			for(j = 0; separatorIndex > 0; ++j)
			{
				separatorIndex = GetCharIndex(line, 20, ' ');
				number = MakeSubstringFromStart(line, separatorIndex, substring1); 
				
				sets[i][j] = atoi(number);
				line = MakeSubstringToEnd(line, separatorIndex + 1, substring2);
			}
		}
		
		fclose ( file );
	}
	else
	{
		PrintErrorCantOpenFile("Dane.txt");
		//Ten wyjatek jest spowodowany przez brak pliku Dane.txt w folderze glownym programu
	}

	return sets;
}

//------------------ Test ------------------//

void TestGetSetsFromFile()
{
	PrintTestStartMessage("GetSetsFromFile");

	int** setsFromFile = GetSetsFromFile();

	int i;
	for(i = 0; i < NUMBER_OF_TRANSACTIONS; ++i)
	{
		PrintTab();

		int j;
		for(j = 0; j < MAX_TRANSACTION_SIZE; ++j)
			printf("%d ", setsFromFile[i][j]);

		PrintNL();
	}

	PrintTestPassedMessage("GetSetsFromFile");
}

//------------------ End of Test ------------------//
