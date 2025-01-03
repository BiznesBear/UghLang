# UghLang
Written in C# interpreted programing language created for educational purpose and to show the power of simplicity.

# Building
Make sure you have .net 9 installed
```
git clone https://github.com/BiznesBear/UghLang
cd UghLang
dotnet build
```

# Running script
```
ughlang master.ugh
```

### Console arguments
- `--debug` (prints debug tree)
- `--version` (prints version)
- `--info` (prints info about the language)
- `--help` (currently
- `--nowarns` (disables warrings)
- `--noexe` (prevents AST from being loaded and executed)

# Mini documentation
This section is dedicated to the basic functions of the ugh language.

## How brackets work in UghLang
> [!NOTE]
> This inconvenience may be removed in the future

() creates new expression which returns somthing.

Don't do:
```
print "Hello " + "world!"; # This won't work, because ugh (currently) doesn't know that there's a calculation #
```
Do:
```
print("Hello " + "world!"); # print keyword prints one single value, not a list of them # 
```

## Keywords list
- print
- input
- free
- true
- false
- fun
- break
- return 
- if
- else
- elif
- repeat
- while
- foreach
- insert
- local
- module (experimental)
- as

### Declaring variables
```ugh
myVar = 5;
print("myVar equals " + myVar);
```

### Releasing resources
```ugh
# TIP: You can replace name with "all" to release all names. #

myVar = 5;
free myVar;
```

### Math operations
```ugh
myVar = 5; # Set #
myVar + 5; myVar += 5;# Add #
myVar - 5; myVar -= 5;# Subtract #
myVar * 5; myVar *= 5;# Multiply #
myVar / 5; myVar /= 5;# Divide #
myVar ** 2; # Power #
myVar // 2; # Square root #

# PI example #
pi = π; # PI number is aviable in std #
# Remember to put comma or any operator between names #
print (2pi, 2pi); # Result: 39,47842 #
```


### If and else and elif
```ugh
if(5 > 3){
	print "5 is higher than 3";
}
elif(5 == 3){
	print "5 equals 3";
}
else {
	print "5 is lower than 3";
}
```

### Repeat
```ugh
myVar = 0;
repeat 10 {
	myVar + 1;
	print myVar;
}
```
### While
```ugh
myVar = 0;
while(myVar < 10){
	myVar + 1;
	print myVar;
}
```
### Foreach
```ugh
myArray = [3];

myArray[0] = "Hello";
myArray[1] = "world";
myArray[2] = "item3";

foreach(item, myArray){
    print item;
}
```


### Fun, return
```ugh
fun Hello(){
	return "Hello";
}

Hello();
print(Hello + " world!"); # Get value from last hello execution #


myVar = Hello();
print(myVar + " world!");

# The fastest way to do all above #
print(Hello() + " world!"); 
```

### Insert
```ugh
insert "std"; # You can put any name of ugh file here (or directory which contains source.ugh) #
Helloworld(); 
```

### Local
```ugh
local fun Foo(){ # Nodes marked as local won't load when inserted from other file # 
	print "Hello, world!";
} 

local { # Example of nested local #

	repeat(100){
		print "Hello, world!";
	}
}
```

### Modules and As keyword
> [!NOTE]
> Modules are experimental future.

```ugh
module "File" as "f";
myVar = f.Read("myFile.txt");
print myVar;
```

### Converting types
```ugh
module "Convert";
myVar = "10";
print(Convert.Int(myVar) + 5);
```

# Examples
## Fibonacci
```ugh
# Fibonacci sequence example #

n = 10; # Lenght #
a = 0; 
b = 1;  

repeat n{ 
    c = (a + b); 
    print c;  
    a = b;     
    b = c;     
}
```