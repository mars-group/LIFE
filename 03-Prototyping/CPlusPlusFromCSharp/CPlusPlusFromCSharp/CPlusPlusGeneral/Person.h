#pragma once
#include <string>

using namespace std;

__declspec(dllexport) void writePersonName();

class Person
{
	string name;
public:
	Person();
	~Person();

};

