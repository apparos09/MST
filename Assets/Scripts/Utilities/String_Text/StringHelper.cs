using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace util
{
    // A helper class for various string related functions.
    public class StringHelper : MonoBehaviour
    {
        // Gets the number of instances of a substring.
        public static int GetSubstringCount(string str, string substr)
        {
            // If the substring is blank.
            if(substr == string.Empty)
            {
                // If the string is also empty, count this as 1 instance.
                if(str == string.Empty)
                {
                    return 1;
                }
                // The string isn't empty, so return a instance count of 0.
                else
                {
                    return 0;
                }
            }

            // If the substring is not in the string at all, return 0.
            if(!str.Contains(substr))
            {
                return 0;
            }

            // OLD - checks each index.
            // // First check if the substring exists in the first place.
            // if(str.Contains(substr))
            // {
            //     // THe index of the current substring instance.
            //     int index = 0;
            // 
            //     // The number of instances.
            //     int instances = 0;
            // 
            //     // While the index is valid.
            //     while(index >= 0 && index < str.Length)
            //     {
            //         // Get the index
            //         index = str.IndexOf(substr, index);
            // 
            //         // If an instance was found.
            //         if(index != -1)
            //         {
            //             // Increase the number of instances.
            //             instances++;
            // 
            //             // Shift the index.
            //             index += substr.Length;
            //         }
            //         else // No instance found.
            //         {
            //             break;
            //         }
            //     }
            // 
            //     // Return the instances count.
            //     return instances;
            // 
            // }
            // else
            // {
            //     return 0;
            // }

            // V2 - uses replace function to compare strings
            
            // Makes a temporary string with all instances of the substring removed.
            string tempStr = str.Replace(substr, "");

            // Gets the difference in length between the two strings.
            int lenDiff = str.Length - tempStr.Length;

            // Calculates the number of instances.
            // Divide by 0 should not be possibly at this stage.
            int instances = lenDiff / substr.Length;

            // Returns the instance count.
            return instances;
        }

    }
}