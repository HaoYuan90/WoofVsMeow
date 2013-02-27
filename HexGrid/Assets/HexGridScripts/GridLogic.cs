using UnityEngine;
using System.Collections.Generic;
using System;

//this class handles logic related to gridmap 
//and is in charge of lighting the grids up 
//(turing masks on and off)

public class GridLogic : MonoBehaviour {
	
	public class Node:IEquatable<Node>, IComparable<Node> {
	    public int X {get; set;}
	    public int Y {get; set;}
	    public float Cost_So_Far {get; set;}
	    public double Cost_Remaining {get; set;}
		public List<List<int>> Past_Nodes {get; set;}
	
    	public Node(int x,int y,float cost_so_far,double cost_remaining, List<List<int>> past_nodes) {
	        this.X=x;
	        this.Y=y;
			this.Cost_So_Far=cost_so_far;
			this.Cost_Remaining=cost_remaining;
			this.Past_Nodes=past_nodes;
			this.Past_Nodes.Add(new List<int>(){x,y});
	    }
		
	    public bool Equals(Node other) {
	        return (this.X==other.X && this.Y==other.Y);
	    }
		
		public Node getTopLeft() {
			Node n = (Y%2==0)?
				new Node(X-1,Y+1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes)):
				new Node(X  ,Y+1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes));
			if (n.isInBounds()) return n; else return null;
		}
		
		public Node getTopRight() {
			Node n = (Y%2==0)?
				new Node(X  ,Y+1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes)):
				new Node(X+1,Y+1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes));
			if (n.isInBounds()) return n; else return null;
		}
		
		public Node getLeft() {
			Node n=
				new Node(X-1,Y  ,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes));
			if (n.isInBounds()) return n; else return null;
		}
		
		public Node getRight() {
			Node n=
				new Node(X+1,Y  ,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes));
			if (n.isInBounds()) return n; else return null;
		}
		
		public Node getBottomLeft() {
			Node n = (Y%2==0)?
				new Node(X-1,Y-1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes)):
				new Node(X,  Y-1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes));
			if (n.isInBounds()) return n; else return null;
		}
		
		public Node getBottomRight() {
			Node n = (Y%2==0)?
				new Node(X,  Y-1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes)):
				new Node(X+1,Y-1,Cost_So_Far+1,Cost_Remaining-1,new List<List<int>>(Past_Nodes));
			if (n.isInBounds()) return n; else return null;
		}
		
		public bool isInBounds() {
			int max_x=GameObject.Find("GridRenderer").GetComponent<GridGenerator>().m_gridNumHor;
			int max_y=GameObject.Find("GridRenderer").GetComponent<GridGenerator>().m_gridNumVer;
			int gridsToDraw = (Y%2==0) ? max_x : max_x-1;
       		return (Y>=0 && Y<max_y && X>=0 && X<gridsToDraw && Cost_Remaining>=0);
		}
		
		public void updateCostRemaining(int x2,int y2) {
			double newX=(Y%2==0)?(double)X:0.5+X;
			double newx2=(y2%2==0)?(double)x2:0.5+x2;
			Cost_Remaining=Math.Sqrt((newX-newx2)*(newX-newx2)+(Y-y2)*(Y-y2));
		} 
			
		public int CompareTo(Node other) {
	        return Cost_Remaining.CompareTo(other.Cost_Remaining);
	    }
		
		public override string ToString() {
			return string.Format("({0}, {1})",X,Y);	
		}
	}
	
	private List<List<GameObject>> m_grids;
	private int click_number=0;
	private int current_grid_x=0;
	private int current_grid_y=0;
	
	private void InitGridData()
	{
		//Look for grid renderer game object in scene and get grid data
		m_grids = GameObject.Find("GridRenderer").GetComponent<GridGenerator>().GetGridData();
	}
	
	private void CheckGridData()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				Debug.Log (b.GetComponent<HexGridModel>());
	}
	
	private void ClearAllMasks()
	{
		foreach(List<GameObject> a in m_grids)
			foreach(GameObject b in a)
				b.GetComponent<MaskManager>().OutlineMaskOn();
	}
	
	// Use this for initialization
	void Start () {
		InitGridData();
		//CheckGridData();
	}
	
	List<Node> getNeighbours(Node c) {
		List<Node> neighbours=new List<Node>();
		Node n;
		n=c.getTopLeft();     if (n!=null) neighbours.Add(n);
		n=c.getTopRight();    if (n!=null) neighbours.Add(n);
		n=c.getLeft();        if (n!=null) neighbours.Add(n);
		n=c.getRight();       if (n!=null) neighbours.Add(n);
		n=c.getBottomLeft();  if (n!=null) neighbours.Add(n);
		n=c.getBottomRight(); if (n!=null) neighbours.Add(n);
		return neighbours;
	}
	
	void HighlightMovementRange(int x, int y, int range) {
		List<Node> open_list=new List<Node>();
		List<Node> closed_list=new List<Node>();
		Node current_node=new Node(x,y,0,range,new List<List<int>>());
		if (current_node.isInBounds())
			open_list.Add(current_node);
		while (open_list.Count!=0) {
			current_node=open_list[0];
			//Debug.Log("Current node: "+current_node.ToString());
			open_list.RemoveAt(0);
			if (!closed_list.Contains(current_node)) {
				closed_list.Add(current_node);
				m_grids[current_node.Y][current_node.X].GetComponent<MaskManager>().GreenMaskOn();
				List<Node> current_neighbours=getNeighbours(current_node);
				foreach (Node n in current_neighbours) {
					if (open_list.Contains(n)) {
						if (n.Cost_Remaining>open_list.Find(n2=>n2.Equals(n)).Cost_Remaining) {
							open_list.Remove(n);
							open_list.Add(n);
						}
					}
					else
						open_list.Add(n);
				}
			}
		}
	}
	
	void HighlightMovementPath(int x1, int y1, int x2, int y2) {
		List<Node> open_list=new List<Node>();
		List<Node> closed_list=new List<Node>();
		Node current_node=new Node(x1,y1,0,0,new List<List<int>>());
		current_node.updateCostRemaining(x2,y2);
		if (current_node.isInBounds())
			open_list.Add(current_node);
		while (open_list.Count!=0) {
			current_node=open_list[0];
			Debug.Log("Current node: "+current_node.ToString());
			open_list.RemoveAt(0);
			if (current_node.X==x2 && current_node.Y==y2)
				break;
			if (!closed_list.Contains(current_node)) {
				closed_list.Add(current_node);
				List<Node> current_neighbours=getNeighbours(current_node);
				foreach (Node n in current_neighbours) {
					n.updateCostRemaining(x2,y2);
					if (open_list.Contains(n)) {
						if (n.Cost_Remaining>open_list.Find(n2=>n2.Equals(n)).Cost_Remaining) {
							open_list.Remove(n);
							int index=open_list.BinarySearch(n);
							if (index<0)
								open_list.Insert(~index,n);
							else
								open_list.Insert(index,n);
						}
					}
					else {
						int index=open_list.BinarySearch(n);
						if (index<0)
							open_list.Insert(~index,n);
						else
							open_list.Insert(index,n);
					}
				}
			}
			current_node=null;
		}
		if (current_node!=null)
			foreach (List<int> l in current_node.Past_Nodes)
			{
				Debug.Log("Past nodes: "+l[0].ToString()+", "+l[1].ToString());
				m_grids[l[1]][l[0]].GetComponent<MaskManager>().BlueMaskOn();
			}
		m_grids[y1][x1].GetComponent<MaskManager>().RedMaskOn();
	}
	
	void Update () {
		if(Input.GetButtonDown("Fire1")) {
			click_number=(click_number+1)%2;
			RaycastHit grid;
			Ray selection = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(selection,out grid)){				
				if (click_number==1) { //BFS
					Debug.Log("Clicking on: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_x.ToString()
										+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_y.ToString());
					current_grid_x=grid.collider.gameObject.GetComponent<HexGridModel>().m_x;
					current_grid_y=grid.collider.gameObject.GetComponent<HexGridModel>().m_y;
					ClearAllMasks();
					HighlightMovementRange(current_grid_x, current_grid_y, 9);
					grid.collider.gameObject.GetComponent<MaskManager>().RedMaskOn();
				}
				else { //A*
					Debug.Log("Destination: "+grid.collider.gameObject.GetComponent<HexGridModel>().m_x.ToString()
										+", "+grid.collider.gameObject.GetComponent<HexGridModel>().m_y.ToString());
					HighlightMovementPath(current_grid_x, current_grid_y,
						grid.collider.gameObject.GetComponent<HexGridModel>().m_x,
						grid.collider.gameObject.GetComponent<HexGridModel>().m_y);
				}
			}
		}
	}
}
