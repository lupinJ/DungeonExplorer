using System.Collections;
using System.Collections.Generic;
using System;

namespace WallOfDefence.Utility
{
    /// <summary>
    /// Unity에서 사용할 수 있는 제네릭 우선순위 큐 구현체 (최소 힙 기반).
    /// TPriority 타입은 IComparable<TPriority>를 구현해야 합니다.
    /// 숫자의 경우, 값이 낮을수록 우선순위가 높습니다.
    /// </summary>
    /// <typeparam name="TElement">큐에 저장할 요소의 타입.</typeparam>
    /// <typeparam name="TPriority">요소의 우선순위 타입.</typeparam>
    public class PriorityQueue<TElement, TPriority>
        where TPriority : IComparable<TPriority>
    {
        // 요소와 우선순위를 저장하는 리스트 (힙 구조를 구현함)
        private readonly List<(TElement Element, TPriority Priority)> _heap = new List<(TElement, TPriority)>();
        Point point;
        /// <summary>
        /// 큐에 있는 요소의 개수를 반환합니다.
        /// </summary>
        public int Count => _heap.Count;

        /// <summary>
        /// 새로운 요소를 큐에 추가합니다.
        /// </summary>
        /// <param name="element">추가할 요소.</param>
        /// <param name="priority">요소의 우선순위.</param>
        public void Enqueue(TElement element, TPriority priority)
        {

            _heap.Add((element, priority));
            SiftUp(_heap.Count - 1);
        }

        /// <summary>
        /// 우선순위가 가장 높은 요소를 제거하고 반환합니다.
        /// </summary>
        /// <returns>우선순위가 가장 높은 요소.</returns>
        /// <exception cref="InvalidOperationException">큐가 비어있는 경우 발생.</exception>
        public TElement Dequeue()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            // 루트 (가장 높은 우선순위) 요소를 저장
            var elementToReturn = _heap[0].Element;

            // 1. 마지막 요소를 루트로 이동
            int lastIndex = _heap.Count - 1;
            _heap[0] = _heap[lastIndex];

            // 2. 마지막 요소 제거
            _heap.RemoveAt(lastIndex);

            // 3. 힙 속성 유지 (Heapify-Down)
            if (Count > 0)
            {
                SiftDown(0);
            }

            return elementToReturn;
        }

        /// <summary>
        /// 우선순위가 가장 높은 요소를 제거하지 않고 반환합니다.
        /// </summary>
        /// <returns>우선순위가 가장 높은 요소.</returns>
        /// <exception cref="InvalidOperationException">큐가 비어있는 경우 발생.</exception>
        public TElement Peek()
        {
            if (Count == 0)
            {
                throw new InvalidOperationException("PriorityQueue is empty.");
            }

            // 루트 (가장 높은 우선순위) 요소를 반환
            return _heap[0].Element;
        }

        /// <summary>
        /// 큐에서 모든 요소를 제거합니다.
        /// </summary>
        public void Clear()
        {
            _heap.Clear();
        }

        // ----------------- 힙 연산 -----------------

        /// <summary>
        /// 부모 노드의 인덱스를 반환합니다.
        /// </summary>
        private int GetParentIndex(int index) => (index - 1) / 2;

        /// <summary>
        /// 왼쪽 자식 노드의 인덱스를 반환합니다.
        /// </summary>
        private int GetLeftChildIndex(int index) => (2 * index) + 1;

        /// <summary>
        /// 오른쪽 자식 노드의 인덱스를 반환합니다.
        /// </summary>
        private int GetRightChildIndex(int index) => (2 * index) + 2;

        /// <summary>
        /// 현재 노드를 부모 노드와 비교하여 올바른 위치로 올립니다 (Enqueue 시 사용).
        /// </summary>
        private void SiftUp(int index)
        {
            int parentIndex = GetParentIndex(index);

            // 현재 노드의 우선순위가 부모 노드보다 높으면 (더 작으면), 교환
            while (index > 0 && _heap[index].Priority.CompareTo(_heap[parentIndex].Priority) < 0)
            {
                Swap(index, parentIndex);
                index = parentIndex;
                parentIndex = GetParentIndex(index);
            }
        }

        /// <summary>
        /// 현재 노드를 자식 노드와 비교하여 올바른 위치로 내립니다 (Dequeue 시 사용).
        /// </summary>
        private void SiftDown(int index)
        {
            int size = Count;

            while (true)
            {
                int leftChildIndex = GetLeftChildIndex(index);
                int rightChildIndex = GetRightChildIndex(index);
                int smallestIndex = index; // 가장 우선순위가 높은 노드의 인덱스

                // 왼쪽 자식과 비교
                if (leftChildIndex < size && _heap[leftChildIndex].Priority.CompareTo(_heap[smallestIndex].Priority) < 0)
                {
                    smallestIndex = leftChildIndex;
                }

                // 오른쪽 자식과 비교 (왼쪽 자식보다 우선순위가 높으면 교체)
                if (rightChildIndex < size && _heap[rightChildIndex].Priority.CompareTo(_heap[smallestIndex].Priority) < 0)
                {
                    smallestIndex = rightChildIndex;
                }

                // 현재 노드가 가장 작지 않으면 (우선순위가 가장 높지 않으면) 교환하고 다음 레벨로 이동
                if (smallestIndex != index)
                {
                    Swap(index, smallestIndex);
                    index = smallestIndex;
                }
                else
                {
                    // 현재 위치가 올바른 위치
                    break;
                }
            }
        }

        /// <summary>
        /// 두 인덱스의 요소를 교환합니다.
        /// </summary>
        private void Swap(int i, int j)
        {
            var temp = _heap[i];
            _heap[i] = _heap[j];
            _heap[j] = temp;
        }
    }
}

