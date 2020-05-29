
# USQL USAC sql

This project consist in a system managment of data base. This consist on two applications that communicate between them throught sockets. 

The first applications is the back end of the system ( database and interpreter), is a engine of data base that allow to the user create relationals data bases that a diference to the tradicionals databases have too a lenguage of programations POO in itself that allow to save differents objects definitions and methods and functions in the data base and that can be triggered making transactions in the database. This applications was developed using C# lenguaje of programations and using the kit of implementation of lenguages [Irony](https://archive.codeplex.com/?p=irony). 

The second applications is the front end of the system that connect with the back end across sockets. Is a visual basic applications that make a first lexical and syntatic analysis to the user's input.  This analysis was developed using [Gold parser](http://www.goldparser.org/).

## IDE front end
Integrate devoloper enviarement, that is a program developed in visual basic and allow to the user connects whit the data base engine and make all transaction from here. 

Characteristics:
  - Open files and projects
  - Navegate througth the data bases using a three navegator. 
  - Multiple edition thanks to multitab with text code highlight.   
  - Output console.  
  - Report area of errors. 
  - Reports of symbol table (Report until the last breakpoint setting)  
  - Search and replace of text.    


## Backend: Engine of data base: interpreter of the diferents lenguages. 
To see the specific of desing of lenguage see the [specifications document](/project_document.pdf). 
1. USQL is sql traditional lenguage that was enriched that can save objects similar to the objects of the programation oriented objects and methods and functions in the database.  
2. XML is the lenguage that is used to managment the data system. The data base system is compound for diferents files:
- Master file: Is the file that save the address and informations about all the databases. 
- USer file: Is the file that save information about all users, the permission, your data bases, etc. 
- backup: Carpet that have all the backups that the user make throught the USQL server. Are saved how a zip file each backup.
- Dump: Carpet that have all the dups that the user are maked throught the USQL server. Are saved how .usql file each dump. 
- Data bases: Each data base is saved in a carpet whit the name and have the next structure:

  - users: file that save the users that can access to this data base.
  - objectos: File that have the informations about all the objects of this data base.
  - DB: File that have the definitions of the data base, that is the tables definitions, etc.
  - procedimiento: File that save all the methods and functions that are definited in the data base. 
  - table_files: There are a file with the name of the each table that have all the tupes (registers) that belong to the table. 

There are a example of file syste in the carpet [/example_data_base_system](/example_data_base_system) and example of input usql file in the carpet [/example_file_input](/example_file_input).


## Develop of system
  - Analysis and execution complete of lenguage of programations usql
  - Analysis and execution complete of lenguage of communitacions PlyCS. 
  - Analysis and execution complete of lenguage of data system XML.
  - Support of class and all characteristics of POO.  
  - Lexical, syntactic and semantics analisys complete. 
  - Report of errors. 
  - Report of symbol table by graphics using graphviz. 
  - Report of AST generated. 
  - AST generated using the pattern of desing interpeter. 
  - Using singlenton pattern. 
  - Develped using Irony and golden parser.

# Autor
  Erick Tejaxún
  erickteja@gmail.com

# Licencia 
  MIT 


## Licencia
[MIT](https://choosealicense.com/licenses/mit/)
