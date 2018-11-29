using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyUtility {

	public static int Gcd(int a, int b){
		if (a < b){
			return Gcd(b, a);
		}

		while(b != 0){
			int rem = a % b;
			a = b;
			b = rem;
		}

		return a;
	}
}
