namespace AoC;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

public class Day15
{
    [Theory]
    [InlineData("day15_example.txt", 40)]
    [InlineData("day15.txt", 498)]
    public async Task Part1(string filename, int expectation)
    {
        var lines = await Input.ReadAllLinesAsync(filename);
        var cavernWidth = lines[0].Length;
        var cavernHeight = lines.Length;
        var riskLevels = lines.SelectMany(line => line.Select(c => int.Parse(c.ToString()))).ToArray();

        // Dijkstra's algorithm
        // https://en.wikipedia.org/wiki/Dijkstra's_algorithm

        // Let the node at which we are starting at be called the initial node.
        // Let the distance of node Y be the distance from the initial node to Y.
        // Dijkstra's algorithm will initially start with infinite distances and will try to improve them step by step.
        const int initialNode = 0;
        var currentNode = initialNode;
        var destinationNode = riskLevels.Length - 1;

        // 1. Mark all nodes unvisited. Create a set of all the unvisited nodes called the unvisited set.
        var unvisitedSet = Enumerable.Repeat(true, riskLevels.Length).ToArray();

        // 2. Assign to every node a tentative distance value: set it to zero for our initial node and to infinity for
        // all other nodes. The tentative distance of a node v is the length of the shortest path discovered so far
        // between the node v and the starting node. Since initially no path is known to any other vertex than the
        // source itself (which is a path of length zero), all other tentative distances are initially set to infinity.
        // Set the initial node as current.
        var tentativeDistances = new int?[riskLevels.Length];
        tentativeDistances[initialNode] = 0;

        while (true)
        {
            // 3. For the current node, consider all of its unvisited neighbors and calculate their tentative distances
            // through the current node. Compare the newly calculated tentative distance to the current assigned value
            // and assign the smaller one. For example, if the current node A is marked with a distance of 6, and the
            // edge connecting it with a neighbor B has length 2, then the distance to B through A will be 6 + 2 = 8. If
            // B was previously marked with a distance greater than 8 then change it to 8. Otherwise, the current value
            // will be kept.
            var unvisitedNeighbours = IndicesOfNeighbours(currentNode, cavernWidth, cavernHeight).Where(i => unvisitedSet[i]);
            foreach (var unvisitedNeighbour in unvisitedNeighbours)
            {
                var currentDistance = tentativeDistances[currentNode];
                var newTentativeDistance = currentDistance + riskLevels[unvisitedNeighbour];
                var currentlyAssignedTentativeDistance = tentativeDistances[unvisitedNeighbour];
                if (!currentlyAssignedTentativeDistance.HasValue || newTentativeDistance < currentlyAssignedTentativeDistance)
                {
                    tentativeDistances[unvisitedNeighbour] = newTentativeDistance;
                }
            }

            // 4. When we are done considering all of the unvisited neighbors of the current node, mark the current node
            // as visited and remove it from the unvisited set. A visited node will never be checked again.
            unvisitedSet[currentNode] = false;

            // 5. If the destination node has been marked visited (when planning a route between two specific nodes) or
            // if the smallest tentative distance among the nodes in the unvisited set is infinity (when planning a
            // complete traversal; occurs when there is no connection between the initial node and remaining unvisited
            // nodes), then stop. The algorithm has finished.
            if (unvisitedSet[destinationNode] == false) { break; }

            // 6. Otherwise, select the unvisited node that is marked with the smallest tentative distance, set it as
            // the new current node, and go back to step 3.
            currentNode = unvisitedSet
                .Select((isUnvisited, node) => (node, tentativeDistance: isUnvisited ? tentativeDistances[node] : null))
                .Where(x => x.tentativeDistance.HasValue)
                .MinBy(x => x.tentativeDistance).node;
        }

        Assert.Equal(tentativeDistances[destinationNode], expectation);
    }

    private static IEnumerable<int> IndicesOfNeighbours(int currentIndex, int width, int height)
    {
        var x = currentIndex % width;
        var y = currentIndex / width;

        return new int?[]
        {
            y == 0 ? null : currentIndex - width,
            x == 0 ? null : currentIndex - 1, x == width - 1 ? null : currentIndex + 1,
            y == height - 1 ? null : currentIndex + width,
        }.Where(i => i.HasValue).Cast<int>();
    }
}