using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net;
using System.Web.Script.Serialization;
using HtmlAgilityPack;
using JourneyPlanner.Models.GoogleModels;
using JourneyPlanner.Data;
using JourneyPlanner.Models.LocationModels;
using JourneyPlanner.Models.TimeTableModels;

namespace JourneyPlanner.Models.GraphModels
{
    /// <summary>
    /// The BFS function requires us to store GraphNodes along with the particular line/route that was travelled on, so a PathNode
    /// object wraps a GraphNode up with an easier to get station name (GraphNode.name also contains this data) and a line name.
    /// </summary>
    public class PathNode
    {
        public GraphNode node { get; set; }
        public string name { get; set; }
        public string line { get; set; }

        public PathNode(string station, string line, GraphNode node)
        {
            this.name = station;
            this.line = line;
            this.node = node;
        }
    }

    /// <summary>
    /// GraphNodes contain data for stations/stops and are used in the Graph structure which forms an abstraction of the public transport
    /// network. Each node has a (station) name, a List of the lines that the station is part of (unused at present) and a Dictionary containing
    /// references to the next and previous station on the line, for each line that the station is part of (perhaps 2 or 3, probably not more than
    /// that).
    /// </summary>
    public class GraphNode
    {
        public string name { get; set; } // the name of the node
        public List<string> lines; // what lines is this station part of (ie a train line and a bus route)
        public Dictionary<string, Dictionary<string, station>> adjacency_list; // <line, <next/prev, station>>

        public GraphNode(station station, string line, station next, station prev)
        {
            name = station.name; // set the name of the node to the name of the station
            lines = new List<string>(); // this list is kinda redundant, but you can see what lines exist in the graph if you access it
            lines.Add(line); // add this line/route to the list of lines/routes that this station is part of
            adjacency_list = new Dictionary<string, Dictionary<string, station>>(); // <line, <next/prev, station>>
            Dictionary<string, station> adj = new Dictionary<string, station>(); // <next = station, prev = station>
            adj.Add("next", next); // "next" = station next
            adj.Add("prev", prev); // "prev" = station prev
            adjacency_list.Add(line, adj);
        }
    }

    /// <summary>
    /// The Graph class takes Lists of station objects and creates a graph structure. Each station is wrapped in a GraphNode object
    /// which contains a moderately complex Dictionary with references to the next and previous station for each line that the station
    /// is part of. For example, some train stations might also form part of a bus route, and we need to allow a transition between
    /// transport modes.
    /// 
    /// The main purpose of Graph is to facilitate path finding algorithms, namely Breadth First Search (slow, non-optimal), but
    /// eventually A* Search (more efficient, shortest path), and perhaps both, as we will get different results from each one and the
    /// assignment asks for multiple journey options. 
    /// </summary>
    public class Graph
    {
        List<GraphNode> graph; // the nodes of the graph, each node is connected by a reference to the next

        public Graph()
        {
            graph = new List<GraphNode>(); // initalise the list
        }

        public void AddLineToGraph(List<station> line, string line_name)
        {
            for (int i = 0; i < line.Count; i++) // for each station on the line
            {
                bool node_exists = false;
                station prev = null, next = null;
                if (i > 0) // if it's not the first node, a previous station exists
                {
                    prev = line[i - 1]; // prev = the previous station in the list
                }
                if (i < (line.Count - 1)) // if it's not the last node, a next station exists
                {
                    next = line[i + 1]; // next = the next station in the list
                }

                // if the node already exists in the graph, we want to add this line/route
                // and these next/prev references to it's adjacency_list rather than
                // add a duplicate node

                foreach (GraphNode n in graph)
                {
                    if (n.name == line[i].name) // if a node exists with this name
                    {
                        Dictionary<string, station> adj = new Dictionary<string, station>(); // new sub-Dict
                        adj.Add("next", next); // set next reference
                        adj.Add("prev", prev); // set prev reference
                        n.adjacency_list.Add(line_name, adj); // add sub-Dict to main-Dict
                        n.lines.Add(line_name); // this list is handy if you just want to see what lines exist for this GraphNode, but isn't used yet
                        node_exists = true; // set a flag
                    }
                }

                // if we didn't find a matching node, ADD A NEW NODE
                if (node_exists == false)
                {
                    GraphNode node = new GraphNode(line[i], line_name, next, prev); // create a new node
                    graph.Add(node); // add it to the graph
                }
            }
        }

        // order of print not necessarily correct, ammended nodes get rearranged in the list
        public void PrintGraph(string line)
        {
            foreach (GraphNode node in graph)
            {
                if (node.adjacency_list.ContainsKey(line)) // if the node is on the specified line/route
                {
                    Console.Write("{0} ", node.name); // print it's name
                    Dictionary<string, station> adj = node.adjacency_list[line]; // grab the next/prev dict
                    foreach (KeyValuePair<string, station> pair in adj)
                    {

                        Console.Write("{0} ", pair.Key); // print "next" / "prev"
                        if (pair.Value != null) // first/last station have no reference for prev/next respectively
                        {
                            Console.Write("{0} ", pair.Value.name); // print the next/prev
                        }
                        else
                        {
                            Console.Write("NONE "); // else print NONE
                        }

                        if (pair.Key == "prev") // if this is the "prev" reference
                        {
                            Console.WriteLine(); // give us a newline
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Perform a Breadth First Search on the graph, given a station to start at, and a destination. This function will always find a path
        /// (and currently prints it out to the console and returns true) if the graph is connected, however, the path is not (guaranteed to be)
        /// optimal.
        /// 
        /// Because Breadth First Search is primarily a search algorithm, and not a path finding algorithm, some extra functionality has been added
        /// to the graph, namely the wrapping of GraphNode objects in PathNode objects, which contain a linked-list style reference from a node to
        /// it's previous node, to enable a backtrace once the destination is found. This backtrace returns the path taken to the node that is found
        /// and is the desired output of this function. The return value needs to adjusted a JSON object that contains this path.
        /// </summary>
        /// <param name="start">The station to start at.</param>
        /// <param name="end">The station to end at.</param>
        /// <returns>At this stage, true = found, false = not found (and generally means an error has occurred, probably in the creation
        /// of the graph structure, and probably with the format of the input data).</returns>
        public List<string> BFS(station start, station end)
        {
            // this dictionary contains PathNode pairs, a node and it's previous node, to determine the path
            // a PathNode contains a GraphNode and the name of the line that was used when passing the node.
            Dictionary<PathNode, PathNode> path = new Dictionary<PathNode, PathNode>();

            // FIRST CHECK IF THE start NODE EXISTS
            GraphNode root = null;
            bool found = false;
            foreach (GraphNode n in graph) // first check if the station is in the graph
            {
                if (n.name == start.name)
                {
                    root = n; // store this node as the root node to begin BFS with
                    found = true; // the station does exist
                }
            }
            if (found == false) // the station does NOT exist, return
                return null;
            // END CHECK IF start NODE EXISTS

            // BFS PROPER
            Queue<PathNode> queue = new Queue<PathNode>(); // pathnode stores the line as well as the node
            List<string> visited = new List<string>(); // this list contains the name of ALL nodes that have been visited.

            PathNode root_pathnode = new PathNode(root.name, "", root);
            queue.Enqueue(root_pathnode); // enqueue the root node (starting point)
            visited.Add(root.name); // and mark it as visited (this list keeps track of visited nodes so they don't get visited again)
            path.Add(root_pathnode, null); // no line given to PathNode just yet

            while (queue.Count > 0) // while the queue is not empty, we still have nodes to check
            {
                PathNode check = queue.Dequeue(); // dequeue the next node

                if (check.node.name == end.name) // if this is the end node, we are done
                {
                    //Console.WriteLine("FOUND IT!! Start {0} End {1}\n", start.name, end.name); // print a message

                    PathNode curr = check, prev; // set curr to the node we found and init prev to null
                    List<string> list_path = new List<string>(); // this list contains the path
                    list_path.Add(curr.line + " - " + curr.name); // add the node we just found as the end of the path
                    while (curr.node != root) // while we are not back at the first node
                    {
                        if (path.TryGetValue(curr, out prev) == true) // found a value matching the key
                        {
                            list_path.Add(curr.line + " - " + prev.name); // add the previous node to the path
                            curr = prev; // set current node to previous node and repeat
                        }
                        else
                        {
                            Console.WriteLine("Couldn't find the prev pathnode, path broken."); // uh oh
                            return null; // we should return if this happens, there's no way to show the path, it shouldn't happen though
                            //break; // finished, we're doomed
                        }
                    }
                    //print the path to console
                    //Console.WriteLine();
                    list_path.Reverse(); // the nodes are back to front, reverse them
                    foreach (string s in list_path)
                    {
                        Console.WriteLine("Path: {0}", s); // print each 'line - station' in the path
                    }
                    return list_path;
                }

                // This part is the main BFS loop, it just keeps going until we find the destination
                // FIND ALL NODES THAT THIS NODE CONNECTS TO AND ADD THEM TO THE QUEUE
                string[] lines = check.node.adjacency_list.Keys.ToArray<string>(); // get all the lines that this node is part of
                foreach (string line in lines)
                {
                    Dictionary<string, station> adj = check.node.adjacency_list[line]; // grab the next/prev dict
                    foreach (KeyValuePair<string, station> pair in adj) // for next, and prev
                    {
                        if (pair.Value != null) // either next or prev exists
                        {
                            foreach (GraphNode n in graph) // find the station in the graph (this sucks, I have to search the whole graph to find the node again)
                            {
                                if (n.name == pair.Value.name && visited.Contains(n.name) == false) // if the node name matches the next/prev name we are checking AND we haven't visited it before
                                {
                                    PathNode to_queue = new PathNode(n.name, line, n); // create a new PathNode with the node name, line and a copy of the GraphNode
                                    queue.Enqueue(to_queue); // enqueue it
                                    visited.Add(n.name); // mark it as visited
                                    found = true; // flag the station does exist
                                    path.Add(to_queue, check); // add the node, and it's previous node to the path so we can retrace steps at the end
                                }
                            }
                            if (found == false) // the station does NOT exist, return false (this is bad and shouldn't happen, means we have bad references)
                                return null;
                        }
                    }
                }
            }
            return null; // not found, again, shouldn't happen if we are searching for a station that exists
        }
    }
}