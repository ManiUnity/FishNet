using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlgoTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    [ContextMenu("BubbleSort")]
    void BubbleSortOp()
    {
        List<int> Datas = new List<int>() { 10, 20, 1, 2, 50, 2, 3, 4 };
        Algo.BubbleSort(Datas,Datas.Count);

        string logdata = string.Join(",", Datas);
        Debug.Log("Bubble Sort "+logdata);

        Datas = new List<int>() { 10, 20, 1, 2, 50, 2, 3, 4 };
        Algo.SelectionSOrt(Datas,0);

        logdata = string.Join(",", Datas);
        Debug.Log("Selection Sort "+logdata);
    }
}


public class Algo
{
    public static void BubbleSort(List<int> Datas,int n)
    {
        if (n <= 1)
            return;
       for (int md = 0; md < Datas.Count - 1; md++)
            {
                if (Datas[md] > Datas[md + 1])
                {
                    Swap(Datas,md, md + 1);
                   
                }
            }
        Debug.Log("N " + n);
        BubbleSort(Datas,n-1);
    }
    /// <summary>
    /// Now find an minimum index 
    /// </summary>
    /// <param name="Datas"></param>
    public static void SelectionSOrt(List<int> Datas,int startIndex)
    {
        int n = Datas.Count;
        // Base case: If we have reached the end of the array, return
        if (startIndex == n - 1)
            return;
        // Find the minimum element in the unsorted part of the array
        int minIndex = MinIndex(Datas, startIndex);
        // Swap the found minimum element with the first element of the unsorted part
        Swap(Datas, startIndex, minIndex);
        // Recursively call SelectionSort on the reduced range
        SelectionSOrt(Datas, startIndex + 1);
    }
    /// <summary>
    /// Find Minimum Index
    /// </summary>
    /// <param name="Datas"></param>
    /// <param name="StartIndex"></param>
    /// <returns></returns>
    public static int MinIndex(List<int> Datas,int StartIndex)
    {
        int minIndex = StartIndex;
         for(int md=StartIndex+1;md<Datas.Count;md++)
        {
            if (Datas[md] < Datas[minIndex])
                minIndex = md;
        }
        return minIndex;
    }
    /// <summary>
    /// Divide left right
    /// individual array and comparison between them lowest insert in filtered array 
    /// </summary>
    public static void MergeSort()
    {

    }
    private static void MergeSortRecursive(List<int> Datas, int left, int right)
    {
        if (left < right)
        {
            int middle = (left + right) / 2;

            // Recursively sort the left and right halves
            MergeSortRecursive(Datas, left, middle);
            MergeSortRecursive(Datas, middle + 1, right);

            // Merge the sorted halves
            Merge(Datas, left, middle, right);
        }
    }

    private static void Merge(List<int> Datas, int left, int middle, int right)
    {
        int n1 = middle - left + 1;
        int n2 = right - middle;

        // Create temporary arrays to hold the two halves
        int[] leftArray = new int[n1];
        int[] rightArray = new int[n2];

        // Copy data to temporary arrays
        for (int i = 0; i < n1; i++)
            leftArray[i] = Datas[left + i];

        for (int j = 0; j < n2; j++)
            rightArray[j] = Datas[middle + 1 + j];

        // Merge the two halves back into the original array
        int iLeft = 0;
        int iRight = 0;
        int k = left;

        while (iLeft < n1 && iRight < n2)
        {
            if (leftArray[iLeft] <= rightArray[iRight])
            {
                Datas[k] = leftArray[iLeft];
                iLeft++;
            }
            else
            {
                Datas[k] = rightArray[iRight];
                iRight++;
            }
            k++;
        }

        // Copy the remaining elements of leftArray, if any
        while (iLeft < n1)
        {
            Datas[k] = leftArray[iLeft];
            iLeft++;
            k++;
        }

        // Copy the remaining elements of rightArray, if any
        while (iRight < n2)
        {
            Datas[k] = rightArray[iRight];
            iRight++;
            k++;
        }
    }
    /// <summary>
    /// Swap of this 
    /// </summary>
    /// <param name="Datas"></param>
    /// <param name="i"></param>
    /// <param name="j"></param>
    public static void Swap(List<int> Datas , int i,int j)
    {
        var temp = Datas[i];
        Datas[i] = Datas[j];
        Datas[j] = temp;
    }
}