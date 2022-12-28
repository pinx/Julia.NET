# Julia.NET

Julia.NET is an API designed to go between .NET and the Julia Language. It utilizes C Interfaces of both languages to allow super efficient transfers between languages (however it does have type conversion overhead as expected). 


## Julia Interface from C#

### Launching Julia
```csharp
JuliaOptions options = new JuliaOptions();
options.ThreadCount = 4;
Julia.Init(options);
```

### Evaluation
```csharp
Julia.Init();
int v = (int) Julia.Eval("2 * 2");
Julia.Exit(0); //Even if your program terminates after you should call this. It runs the finalizers and stuff 
```

### Structs
```csharp

   // You have two choices, allocate a struct or create a struct.
   // Allocating directly sets the memory, creating will call a constructor of the struct
   
   var myAllocatedStruct = Julia.AllocStruct(JLType.JLRef, 3);   //Will throw error
   var myCreatedStuct = JLType.JLRef.Create(3);   //Will call constructor
```

### Functions
```csharp
  JLFun fun = Julia.Eval("t(x::Int) = Int32(x * 2)");
  JLSvec ParameterTypes = fun.ParameterTypes;
  JLType willbeInt64 = fun.ParameterTypes[1];
  JLType willBeInt32 = fun.ReturnType;
  
  JLVal resultWillBe4 = fun.Invoke(2);
```

### Values
```csharp
   //Auto alloc to Julia
   var val = new JLVal(3);

   //Manual Type Unboxing
   long netVal = val.UnboxInt64();
   
   //Auto Unboxing
   object newVal2 = val.Value;
```

### Arrays
```csharp
   JLArray arr = Julia.Eval("[2, 3, 4]")
   
   //Unpack to .net
   object[] o = arr.UnboxArray();
   
   var a = new int[]{2, 3, 4};
   
   //Copy to a Julia Array. Don't use this method if you know an object is an array though. There are faster methods!
   var v = new JLVal(a);
   
   //Fast Array Copy From .NET. This will deal with direct memory transfer rather then boxing/unboxing for unmanaged types
   var v2 = JLArray.CreateArray(a);
   
   //Fast Array Copy From Julia. This will deal with direct memory transfer rather then boxing/unboxing for unmanaged types
   int[] v2 = v2.UnboxArray<int>();
   
   JLType elementType = arr.ElType;
```

### Exception Handling
```csharp
  JLFun fun = Julia.Eval("t(x) = sqrt(x)");
  fun.Invoke(5).Println();   //Exception Checking
  fun.UnsafeInvoke(5).Println();   //No Exception Checking
  Julia.Exit(0);  
```


### Garbage Collection
You are (at the current moment of this project) responsible for ensuring object safety on both .NET and Julia. When you make calls to either language, the GC could activate and invalidate the reference you hold in the other language unless you pin it!

There are two forms of Garbage Collector Pinning: Static & Stack.

Static pinning is meant for objects with a long life span (could exist forever).
Stack pinning is meant for objects with a short life span.


#### CSharp Static Garbage Collector Pinning
```csharp
  JLArray myArr = new JLArray(JLType.Int64, 5);  //Allocate Int64 array of length 5
  
  var handle = myArr.Pin();    //Pin the Object 
  
  //Stuff calling Julia Functions
  
  handle.Free();   //Optional, handle destructor will auto call it. This is in case you want it freed earlier
```

#### CSharp Stack Garbage Collector Pinning
```csharp
    JLVal v = Julia.Eval("2 * 2");
    JLVal v2 = Julia.Eval("Hi");
    Julia.GC_PUSH(v, v2);

    //Do Stuff with v without it being collected

    Julia.GC_POP();    
```


## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.


## Author

Library Written by Johnathan Bizzano
