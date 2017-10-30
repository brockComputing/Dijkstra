using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Dijkstra
{
   
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }

        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1; // child index; start at end
            while (ci > 0)
            {
                int pi = (ci - 1) / 2; // parent index
                if (data[ci].CompareTo(data[pi]) >= 0) break; // child item is larger than (or equal) parent so we're done
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }

        public T Dequeue()
        {
            // assumes pq is not empty; up to calling code
            int li = data.Count - 1; // last index (before removal)
            T frontItem = data[0];   // fetch the front
            data[0] = data[li];
            data.RemoveAt(li);

            --li; // last index (after removal)
            int pi = 0; // parent index. start at front of pq
            while (true)
            {
                int ci = pi * 2 + 1; // left child index of parent
                if (ci > li) break;  // no children so done
                int rc = ci + 1;     // right child
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0) // if there is a rc (ci + 1), and it is smaller than left child, use the rc instead
                    ci = rc;
                if (data[pi].CompareTo(data[ci]) <= 0) break; // parent is smaller than (or equal to) smallest child so done
                T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp; // swap parent and child
                pi = ci;
            }
            return frontItem;
        }

        public T Peek()
        {
            T frontItem = data[0];
            return frontItem;
        }

        public int Count()
        {
            return data.Count;
        }

        public override string ToString()
        {
            string s = "";
            for (int i = 0; i < data.Count; ++i)
                s += data[i].ToString() + " ";
            s += "count = " + data.Count;
            return s;
        }

        public bool IsConsistent()
        {
            // is the heap property true for all data?
            if (data.Count == 0) return true;
            int li = data.Count - 1; // last index
            for (int pi = 0; pi < data.Count; ++pi) // each parent index
            {
                int lci = 2 * pi + 1; // left child index
                int rci = 2 * pi + 2; // right child index

                if (lci <= li && data[pi].CompareTo(data[lci]) > 0) return false; // if lc exists and it's greater than parent then bad.
                if (rci <= li && data[pi].CompareTo(data[rci]) > 0) return false; // check the right child too.
            }
            return true; // passed all checks
        } // IsConsistent
    } // PriorityQueue
    class QueueItem : IComparable<QueueItem>
    {
        public int distance;
        public string cityName;

        public QueueItem(int distance , string cityName)
        {
            this.distance = distance;
            this.cityName = cityName;

        }
        public int CompareTo(QueueItem other)
        {
            if (this.distance  < other.distance) return -1;
            else if (this.distance > other.distance) return 1;
            else return 0;
        }
        public override string ToString()
        {
            return "(" + cityName  + ", " + distance .ToString() + ")";
        }
    }
    

    class Program
    {
        static void Main(string[] args)
        {
            // This is based on the example exercise in the heathcote book.
            // The priority queue code came from https://visualstudiomagazine.com/Articles/2012/11/01/Priority-Queues-with-C.aspx
            // It uses a binary heap which is beyond the scope for A level but worth looking up if you are interested
            // The use of a dictionary was a bit of a hack so the adajacancy list and the priority queue can be tied together.
            // A vid showing the example https://www.youtube.com/watch?v=LcoRa4F6Bcs&list=PLpD4PfFMR3GBlrOyJ5H-TfASjNDIYZ4w5
            
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            int[,] graph = new int[,]
            {/*   0   1   2   3   4         */
           /*0*/{ 0,  34, 75, 74, 0 }, /*Liverpool  */
           /*1*/{ 34, 0 , 45, 39, 69},/*Manchester  */
           /*2*/{ 75, 45, 0,  36, 23},/*Leeds  */
           /*3*/{ 74, 39, 36, 0,  61},/*Sheffield  */
           /*4*/{ 0,  69, 23, 61, 0 }, /*York  */
            };
            bool[] visited = new bool[graph.GetUpperBound(0) + 1];
            dictionary.Add("Liverpool", 0); // i could of used an array here 
            dictionary.Add("Manchester", 1);// an array would of been simpler but i wanted to demonstrate 
            dictionary.Add("Leeds", 2); // a dictionary
            dictionary.Add("Sheffield", 3);
            dictionary.Add("York", 4);
            QueueItem liverpool = new QueueItem(0, "Liverpool");
            QueueItem manchester = new QueueItem(int.MaxValue, "Manchester");
            QueueItem leeds = new QueueItem(int.MaxValue, "Leeds");
            QueueItem sheffield = new QueueItem(int.MaxValue, "Sheffield");
            QueueItem york = new QueueItem(int.MaxValue, "York");
            QueueItem U;
            PriorityQueue<QueueItem> pq = new PriorityQueue<QueueItem>() ;
            pq.Enqueue(liverpool);
            pq.Enqueue(manchester);
            pq.Enqueue(leeds);
            pq.Enqueue(sheffield);
            pq.Enqueue(york);
           
            //Console.WriteLine(pq.ToString() );
            //int[] distance = new int[5];
            bool[] shortestPathTreeSet = new bool[graph.GetUpperBound(0) + 1];
            int[] pathVia = new int[graph.GetUpperBound(0) + 1];
            bool distanceUpdated = false;
            while (Convert.ToInt32( pq.Count()) != 0)
            {
                U = pq.Dequeue();
                int Udistance = U.distance;
                int rowNo = dictionary[U.cityName]; // get the row of the highest priority
                shortestPathTreeSet[rowNo] = true;
                Console.WriteLine(U.cityName + "---" + U.distance);
                for (int column = 0; column < graph.GetUpperBound(0) + 1; column++) // get all if U's neighbours and update their distances
                {
                    if (graph[rowNo,column] != 0 && shortestPathTreeSet[column] == false  ) // if an edge
                    {
                        int distance = graph[rowNo, column];
                        upDateDistances(column   ,pq, dictionary, distance, Udistance, ref distanceUpdated);
                        if (distanceUpdated)
                        {
                            pathVia[column ] = rowNo ; // stores the path back to the start [0]
                        }
                        //              
                    }
                }
            }
            AskUserFromLiverpool(dictionary, pathVia);
            Console.ReadLine();
        }

        private static void AskUserFromLiverpool(Dictionary<string, int> dictionary, int[] pathVia)
        {
            string[] city = new string[dictionary.Count]; 
            Console.WriteLine("Select a number for wich city you want to travel to from Liverpool");
     
            foreach (var item in dictionary)
            {
              
                Console.WriteLine(item.Value + "-" + item.Key);
                city[item.Value] = item.Key;
            }
            int index = Convert.ToInt32(Console.ReadLine());
            while (index != 0)
            {
                index = pathVia[index];
                Console.WriteLine(city[index]);
            }
        }

        private static void upDateDistances(int rowNo, PriorityQueue<QueueItem> pq, Dictionary<string, int> dictionary, int distance, int Udistance, ref bool distanceUpdated)
        {
            // get the city name from the row number
            // dequeue it see if it is the one to update if not enqueue it, if so update the distance and enqueue it.
            Stack<QueueItem> tempStack = new Stack<QueueItem>(); // stack required as all have the same priority at the start.
            string cityname = ""; 
            foreach (var pair in dictionary)
            {
                if (pair.Value == rowNo )
                {
                    cityname = pair.Key;
                    break;
                }
            }
            bool found = false;
            distanceUpdated = false;
            while (!found)
            {
                QueueItem q = pq.Dequeue(); 
                tempStack.Push(q); // save it
                if (cityname == q.cityName)
                {
                    found = true;
                    if (q.distance > (distance + Udistance))
                    {
                        q.distance = distance + Udistance; // up date the distance 
                        distanceUpdated = true;
                    }
                }
            }
            while (tempStack.Count != 0) // put items back in the queue
            {
                pq.Enqueue(tempStack.Pop());
            }
        }

      

       
       

       
    }
}
