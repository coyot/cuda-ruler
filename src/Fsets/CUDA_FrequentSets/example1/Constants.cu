const int NUMBER_OF_TRANSACTIONS = 1000; //number of transactions definied in Dane.txt file
const int MAX_TRANSACTION_SIZE = 10; //maximum number of items in transaction in Dane.txt file
const int MAX_FILE_LINE_SIZE = 50; //maximum size (number of chars) of Dane.txt line

const int NUMBER_OF_DIFFERENT_ITEMS = 20; //Number of different items in database (see GetDefiniedItems.cu file)

const int MIN_SUP = 4; //minimum suppremum of frequent set

const int NUMBER_OF_BITS_IN_INT = 32;

int NUMBER_OF_INTS_IN_ROW = 0; //global variable set in example1.cu file

static const unsigned char LOOK_UP_TABLE_BIT_COUNT[] = 
{
  0, 1, 1, 2, 1, 2, 2, 3, 1, 2, 2, 3, 2, 3, 3, 4, 
  1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 
  1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 
  2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 
  1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 
  2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 
  2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 
  3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 
  1, 2, 2, 3, 2, 3, 3, 4, 2, 3, 3, 4, 3, 4, 4, 5, 
  2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 
  2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 
  3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 
  2, 3, 3, 4, 3, 4, 4, 5, 3, 4, 4, 5, 4, 5, 5, 6, 
  3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 
  3, 4, 4, 5, 4, 5, 5, 6, 4, 5, 5, 6, 5, 6, 6, 7, 
  4, 5, 5, 6, 5, 6, 6, 7, 5, 6, 6, 7, 6, 7, 7, 8
};
/* To generate the table algorithmically use:
LOOK_UP_TABLE_BIT_COUNT[0] = 0;
for (int i = 0; i < 256; i++)
{
  LOOK_UP_TABLE_BIT_COUNT[i] = (i & 1) + LOOK_UP_TABLE_BIT_COUNT[i / 2];
}
*/


//------------------ Test ------------------//

void TestConstants()
{
	PrintTestStartMessage("Constants");

	PrintConstantInt("NUMBER_OF_TRANSACTIONS", NUMBER_OF_TRANSACTIONS);
	PrintConstantInt("MAX_TRANSACTION_SIZE", MAX_TRANSACTION_SIZE);
	PrintConstantInt("MAX_FILE_LINE_SIZE", MAX_FILE_LINE_SIZE);
	PrintConstantInt("NUMBER_OF_DIFFERENT_ITEMS", NUMBER_OF_DIFFERENT_ITEMS);
	PrintConstantInt("MIN_SUP", MIN_SUP);

	PrintTestPassedMessage("Constants");
}

//------------------ End of Test ------------------//