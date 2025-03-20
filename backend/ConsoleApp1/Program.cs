using System.Text;

public class ListNode
{
    public int val;
    public ListNode next;
    public ListNode(int val = 0, ListNode next = null)
    {
        this.val = val;
        this.next = next;
    }
}

public class Solution
{
    public ListNode AddTwoNumbers(ListNode l1, ListNode l2)
    {
        StringBuilder string1 = new();
        StringBuilder string2 = new();

        while (l1.next is not null)
        {
            string1.Insert(0, l1.val);
            l1 = l1.next;
        }

        while (l2.next is not null)
        {
            string1.Insert(0, l2.val);
            l2 = l2.next;
        }
        var f1 = Convert.ToInt32(string1);
        var f2 = Convert.ToInt32(string2);

        var res = f1 + f2;
        StringBuilder string3 = new();
        foreach (char l in res.ToString())
        {
            string3.Insert(0, l);
        }
        ListNode result = new ListNode(); // Head node
        ListNode current = result; // Temporary pointer to keep track of the current node

        foreach (char l in string3.ToString())
        {
            current.val = Convert.ToInt32(l); // Set the value of the current node

            // Only create the next node if we're not at the last node in the list
            if (l != string3.ToString().Last())
            {
                current.next = new ListNode(); // Create the next node
                current = current.next; // Move the pointer to the next node
            }
        }
        return result;
    }
}