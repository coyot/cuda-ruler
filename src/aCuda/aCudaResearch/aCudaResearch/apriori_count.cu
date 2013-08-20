#include <iostream>

extern "C" __global__ void count_frequency(int * input, int * output, unsigned width, unsigned height) 
{
	int baseX = blockIdx.x * blockDim.x + threadIdx.x;
	int totalThreads = blockDim.x * gridDim.x;

	for(int elementIndex = baseX; elementIndex < width; elementIndex += totalThreads) 
	{
		int i = elementIndex;
		int sum = 0;

		while(i < width * height) 
		{
			sum += input[i];
			i += width;
		}

		output[elementIndex] = sum;
	}
}

extern "C" __global__ void count_sets_frequencies(int * input, int * inputSets, int * output, 
												 unsigned width, unsigned height, unsigned setWidth, unsigned sets, unsigned minSup) 
{
	int baseX = blockIdx.x * blockDim.x + threadIdx.x;
	int totalThreads = blockDim.x * gridDim.x;

	for(int cid = baseX; cid < sets; cid += totalThreads) 
	{
		int occuredSum = 0;
		int startPoint = cid * setWidth;

		for(int tid = 0; tid < width; tid++) 
		{
			bool yes = true;
			int i = 0;

			while(i < setWidth) 
			{
				if (input[inputSets[i + startPoint] * width + tid] != 1)
				{
					yes = false;
					i = setWidth;
				} 
				else 
				{
					i++;	
				}
			}

			// we found in the transaction all elements from the checked set
			if(yes) 
			{
				occuredSum++;	
			}

			if(occuredSum > minSup)
					break;
		}

		output[cid] = occuredSum;
	}
}
/*
extern "C" __global__ void count_frequency_matrix(int * input, int * inputSets, int * output, 
												 unsigned width, unsigned height, unsigned setWidth, unsigned sets) 
{
	int baseX = blockIdx.x * blockDim.x + threadIdx.x;
	int totalThreads = blockDim.x * gridDim.x;

	for(int c = 0; c < sets; c++) 
	{
		int occuredSum = 0;

		for(int tid = baseX; tid < height; tid += totalThreads) 
		{
			bool yes = true;

			for(int i = 0; i < setWidth; i++)
			{
				if (input[tid * width + inputSets[i + c * setWidth]] != 1)
				{
					yes = false;
					break;
				}
			}
			// we found in the transaction all elements from the checked set
			if(yes) 
			{
				occuredSum++;	
			}
		}

		output[baseX + c * totalThreads] = occuredSum;
	}
}

extern "C" __global__ void count_frequency_matrix2(int * input, int * inputSets, int * output, 
												 unsigned width, unsigned height, unsigned setWidth, unsigned sets) 
{
	int baseX = blockIdx.x * blockDim.x + threadIdx.x;
	int totalThreads = blockDim.x * gridDim.x;


		for(int tid = baseX; tid < height; tid += totalThreads) 
		{	
			for(int c = 0; c < sets; c++) 
			{
				int occuredSum = output[baseX + c * totalThreads];

				bool yes = true;

				for(int i = 0; i < setWidth; i++)
				{
					if (input[tid * width + inputSets[i + c * setWidth]] != 1)
					{
						yes = false;
						break;
					}
				}
				// we found in the transaction all elements from the checked set
				if(yes) 
				{
					occuredSum++;	
					output[baseX + c * totalThreads] = occuredSum;
				}
			}
		}

}*/

// count sum for each column (which is support for candidate!)
extern "C" __global__ void count_frequency_table(int * input, int * output, 
												 unsigned width, unsigned height) 
{
	int baseX = blockIdx.x * blockDim.x + threadIdx.x;
	int totalThreads = blockDim.x * gridDim.x;

	for(int id = baseX; id < height; id += totalThreads) 
	{
		int sum = 0;
		int innerId = 0;

		while(innerId < width) 
		{
			sum += input[innerId + id * width];
			innerId += 1;
		}

		output[id] = sum;
	}
}