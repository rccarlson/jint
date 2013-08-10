/// Copyright (c) 2012 Ecma International.  All rights reserved. 
/**
 * @path ch15/15.4/15.4.4/15.4.4.19/15.4.4.19-9-4.js
 * @description Array.prototype.map doesn't visit expandos
 */


function testcase() {

  var callCnt = 0;
  function callbackfn(val, idx, obj)
  {
    callCnt++;
  }
  var srcArr = [1,2,3,4,5];
  srcArr["i"] = 10;
  srcArr[true] = 11;

  var resArr = srcArr.map(callbackfn);
  if(callCnt == 5)
  {
    return true;
  }

 }
runTestCase(testcase);