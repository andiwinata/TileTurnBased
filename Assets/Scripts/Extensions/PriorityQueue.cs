using UnityEngine;
using System;
using System.Collections.Generic;

public class PriorityQueue<T> 
{
	private MinBinaryHeap<ItemPriorityKey<T>> elements = new MinBinaryHeap<ItemPriorityKey<T>>();

	public int Count
	{
		get { return elements.Count; }
	}
	
	public void Enqueue(float priority, T item)
	{
		if (!CheckIfExist(item)) //only add if the item is not exist
			elements.Insert(new ItemPriorityKey<T>(priority, item));
	}
	
	public T Dequeue()
	{
		ItemPriorityKey<T> best = elements.PopFirst();
		T bestItem = best.item;

		return bestItem;
	}

	public void ReduceAllPriorityBy(float reduct)
	{
		List<ItemPriorityKey<T>> items = elements.GetData();
		foreach (ItemPriorityKey<T> item in items)
		{
			item.ReducePriorityBy(reduct);
		}
	}

	public void RemoveItem(T item)
	{
		List<ItemPriorityKey<T>> items = elements.GetData();
		foreach (ItemPriorityKey<T> obj in items)
		{
			//replace the old item if found
			if (obj.item.Equals(item))
			{
				items.Remove(obj);
				break;
			}
		}
	}

	public void ReplaceItem(T oldItem, T newItem)
	{
		List<ItemPriorityKey<T>> items = elements.GetData();
		foreach (ItemPriorityKey<T> item in items)
		{
			//replace the old item if found
			if (item.item.Equals(oldItem))
			{
				item.ReplaceItem(newItem);
			}
		}
	}

	private bool CheckIfExist(T obj)
	{
		List<ItemPriorityKey<T>> items = elements.GetData();
		foreach (ItemPriorityKey<T> it in items)
		{
			if (it.item.Equals(obj))
				return true;
		}

		return false;
	}
}
