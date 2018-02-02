using UnityEngine;
using System;
using System.Collections.Generic;

public class ItemPriorityKey<T> : IComparable<ItemPriorityKey<T>>
{
	public float priority { get; private set;}
	public T item { get; private set;}

	public ItemPriorityKey(float keyParam, T itemParam)
	{
		priority = keyParam;
		item = itemParam;
	}

	public int CompareTo(ItemPriorityKey<T> obj)
	{
		float key2 = obj.priority;
		return Mathf.FloorToInt(priority - key2);
	}

	public void ReducePriorityBy (float val)
	{
		priority -= val;
	}

	public void ReplaceItem(T newItem)
	{
		item = newItem;
	}
}
