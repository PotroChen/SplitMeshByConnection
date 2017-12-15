using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour {

    #region PUBLIC_VARIABLES

    #endregion

    #region PRIVATE_VARIABLES

    #endregion

    #region MONOVEHAVIOURS_FUNCTION

    // Use this for initialization
    void Start() {
        int[] test1 = new int[] { 1, 3, 4, 2, 6, 4};
        //test1={( 1, 3, 4, 2, 6, 4)};
        List<int> test2 = new List<int>(test1);
        test2.Sort();
        foreach (var item in test2)
        {
            Debug.Log(item);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

	#endregion

	#region PUBLIC_FUNCTION

	#endregion

	#region PRIVATE_FUNCTION

	#endregion
}
