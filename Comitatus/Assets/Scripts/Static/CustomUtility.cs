using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CustomUtility {

    /// <summary>
    /// Returns a point in a range defined by the divisor. 
    /// Example: Range 0-3 and divisor is 3. Lower returns 1 (the lower third). Upper Returns 2 (the upper third).
    /// </summary>
    /// <param name="rangeMin">Defines the low end of the range.</param>
    /// <param name="rangeMax">Defines the high end of the range.</param>
    /// <param name="divisor">The fractional distance between the difference of max and min.</param>
    /// <param name="upper">If true, returns result relative to max, false returns relative to min.</param>
    public static float GetFracPointFromRange(float rangeMin, float rangeMax, float divisor, bool upper) {
        float value = (rangeMax - rangeMin) / divisor;
        float result;

        if (upper) {
            result = rangeMax - value;
        } else {
            result = rangeMin + value;
        }
        
        return result;
    }

}
