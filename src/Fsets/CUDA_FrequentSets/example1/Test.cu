void PrintTestStartMessage(char *functionName)
{
	printf("-> Test of %s started...\n", functionName); 
}

void PrintTestPassedMessage(char *functionName)
{
	printf("<> Test of %s passed.\n\n", functionName); 
}

void PrintConstantInt(char* name, const int value)
{
	printf("\t%s: %d\n", name, value);
}

__int64 GetTicks()
{
  __int64 Result;
  __asm
  {
      rdtsc
      mov dword ptr [Result],eax
      mov dword ptr [Result+4],edx
  }
  return Result;
}

void PrintInt64(__int64 number)
{
	printf("%I64d", number);
}

void PrintTicks(__int64 startTicks, __int64 stopTicks)
{
	__int64 subtraction = stopTicks - startTicks;
	PrintInt64(subtraction);
}
