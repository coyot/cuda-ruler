//This function must be run before other operations can begin
void InitVariables()
{
	NUMBER_OF_INTS_IN_ROW = NUMBER_OF_TRANSACTIONS / NUMBER_OF_BITS_IN_INT;
	if( (NUMBER_OF_TRANSACTIONS % NUMBER_OF_BITS_IN_INT) != 0 )
		++NUMBER_OF_INTS_IN_ROW;
}
