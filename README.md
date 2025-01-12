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
- `--debug` (enters debug mode)
- `--version` (prints version)
- `--info` or `--help` (displays info about the language)
- `--nowarns` (disables warnings)
- `--stacktrace` (enables stack trace when c# exception is thrown)
- `--noexe` (prevents AST from being loaded and executed)

# Mini documentation
This section is dedicated to the basic functions of the ugh language.

## How brackets work in UghLang
Brackets creates new expression which returns somthing (0 by defalut).

Don't do:
```ugh
print "Hello " + "world!" # This won't work, because ugh (currently) doesn't know that there's a calculation #
```
Do:
```ugh
print("Hello " + "world!") # print keyword prints one single value, not a list of them # 
```

## How semicolons work in UghLang
In old UghLang you needed to put semicolon after ending every instruction. Example:
```ugh
print "Hello, world!";
```
But new versions allows you to be lazy and not put it up after end of the line. Example:
```ugh
print "Hello, world"
```
Simicolons are optional in new lexing system.

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
- module 
- as
- const

### Declaring variables
```ugh
myVar = 5
print("myVar equals " + myVar)
```

### Releasing resources
```ugh
# TIP: You can replace name with "all" to release all names. #
myVar = 5
free myVar
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
pi = π # PI number is aviable also in std as `PI` #
# Remember to put comma or any operator between names. Comma works as * in this example #
print (2pi, 2pi) # Result: 39,47842 #
```

### If and else and elif
```ugh
if(5 > 3){
	print "5 is higher than 3"
}
elif(5 == 3){
	print "5 equals 3"
}
else {
	print "5 is lower than 3"
}
```

### Repeat
Repeats somthing X times and always changes i variable by 1 or -1.

```ugh
# Basic repeat #
repeat 0, 10 { # This will repeat 10 times #
	print "Hello"
}

# Counts from 0 to 10 #
repeat 0, 10, i {
	print i
}

# Reversed repeat. Counts from 10 to 0 #
repeat 10, 0, i {
	print i
}


```

### While
```ugh
myVar = 0;
while(myVar < 10) {
	myVar + 1
	print myVar
}
```

### Foreach
```ugh
# 1 #
myArray = [3]

myArray[0] = "foo"
myArray[1] = "somthing"
myArray[2] = "trash"

foreach item, myArray {
    print item
}

# 2 #
myArray = { "foo", "somthing", "trash" } # Quick array initialization #
foreach item, myArray {
    print item
}
```

### Fun, return
```ugh
fun Hello(){
	return "Hello"
}

Hello();
print(Hello + " world!") # Get value from last hello execution #


myVar = Hello();
print(myVar + " world!")

# The fastest way to do all above #
print(Hello() + " world!")
```

### Insert
```ugh
insert "std" # You can put any name of ugh file here (or directory which contains source.ugh) #
Helloworld()
```

### Local
```ugh
local fun Foo(){ # Nodes marked as local won't load when inserted from other file # 
	print "Hello, world!"
} 

local { # Example of nested local #

	repeat 100 {
		print "Hello, world!"
	}
}
```

### Constants
```ugh
const MYNUM = 100 
MYNUM += 900 # This operation is readonly #
```

### Modules and As keyword
> [!NOTE]
> Modules are experimental future.

```ugh
module "File" as f
myVar = f.Read("myFile.txt")
print myVar
```

### Converting types
```ugh
module "Convert"
myVar = "10"
print(Convert.Int(myVar) + 5)
```

# Examples
## Fibonacci
```ugh
# Fibonacci sequence example #

n = 10 # Lenght #
a = 0 
b = 1  

repeat 0, n { 
    c = (a + b) 
    print c
    a = b 
    b = c    
}
```