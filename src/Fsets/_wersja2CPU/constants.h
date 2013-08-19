#ifndef CONSTANTS_H
#define CONSTANTS_H

const int itemsetCount=131072; 
const int distinctItemCount=10000;
const int frequentItemsetSize=5;
const int maxRecursion=frequentItemsetSize;
const int frequentItemsetsToGenerate=50;
const float minsupPct=0.1;
const int minsup=(int)round((itemsetCount*minsupPct));
//const int minsup=5;
const int rowSizeInBytes=(itemsetCount>>3)+((itemsetCount&0x07)!=0?1:0); //Podziel przez 8 zakraglajac w gore
const int rowSizeIntMultiple=(rowSizeInBytes>>2)+((rowSizeInBytes&0x03)!=0?1:0); //Podziel przez 4 zaokraglajac w gore. Wynik musi być potęgą 2!
const int rowSizeExtendedInBytes=rowSizeIntMultiple*sizeof(int);//pisze sizeof(int), ale wzory uwzgledniaja 32 bity ;P )
const int dataTableSizeInBytes=rowSizeExtendedInBytes*distinctItemCount;

#endif

