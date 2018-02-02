using UnityEngine;
using System;
using System.Collections.Generic;

public class MinBinaryHeap<T> where T: IComparable<T>
{	//NOTE: use XOR to reverse to max (havent implemented yet)
	List<T> items;

	public T firstItem
	{
		get
		{return items[0];}
	}

	public int Count
	{
		get
		{return items.Count;}
	}

	public MinBinaryHeap()
	{
		items = new List<T>();
	}

	public void Insert(T obj)
	{
		//add the object to end of list
		items.Add(obj);

		//--SORT bottom to top--
		//get the last index of the item
		int i = items.Count - 1;

		while (i > 0) //if index is not the first index
		{
			int parent = (i - 1) / 2; //get parent index

			//compare it to parent if it is smaller
			if (items[i].CompareTo(items[parent]) < 0)
			{
				//swap parent and child
				T temp = items[parent];
				items[parent] = items[i];
				items[i] = temp;

				//change the index
				i = parent;
			}
			else //if it is bigger than parent
			{
				break; //get out and done
			}
		}
	}

	//remove first index
	public void RemoveFirst()
	{
		int i = items.Count - 1; //get the last index
		items[0] = items[i]; //swap first to last
		items.RemoveAt(i); //remove the last

		i = 0; //update index

		while (true)
		{
			//--SORT top to bottom--
			int leftId = 2 * i + 1; //left branch index
			int rightId	= 2 * i + 2; //right branch index
			
			int smallestId = i; //smallest get to the top

			//compare parent to left child and right child
			//if item at smallestId is larger than left child, swap it //if it the same value, still swap it, let the first enter become first out
			if (leftId < items.Count && items[smallestId].CompareTo(items[leftId]) >= 0) 
			{
				smallestId = leftId;
			}
			//if item at smallestId is larger than right child, swap it //if it the same value, still swap it, let the first enter become first out
			if (rightId < items.Count && items[smallestId].CompareTo(items[rightId]) >= 0) 
			{
				smallestId = rightId;
			}

			//swap position if smallestId different
			if (smallestId != i)
			{
				T temp = items[i];
				items[i] = items[smallestId];
				items[smallestId] = temp;

				//go search again with new index
				i = smallestId;
			}
			else //i is the smallest
			{
				break;
			}

		}
	}

	public T PopFirst()
	{
		T t = items[0];

		RemoveFirst();
		return t;
	}

	public List<T> GetData()
	{
		return items;
	}
}
