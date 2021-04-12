using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;


    public class tk_lazer
    {
        // A Test behaves as an ordinary method
        [Test]
        public void tk_lazerSimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator tk_lazerWithEnumeratorPasses()
        {
        
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }

