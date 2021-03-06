#define INT_MAX 2147483647
#define INT_MIN -2147483648
#define SHORT_MAX 32767
#define SHORT_MIN -32768
#define LONG_MAX 2147483647
#define LONG_MIN -2147483648
#define BYTE_MAX 127
#define BYTE_MIN -128

#define UINT_MAX 4294967295
#define USHORT_MAX 65535
#define ULONG_MAX 4294967295

#define I8 "127"
#define I16 "32767"
#define I32 "2147483647"
#define I64 "9223372036854775807"
#define I128 "170141183460469231731687303715884105727"
#define I256 "57896044618658097711785492504343953926634992332820282019728792003956564819967"
#define I512 "6703903964971298549787012499102923063739682910296196688861780721860882015036773488400937149083451713845015929093243025426876941405973284973216824503042047"
#define I1024 "89884656743115795386465259539451236680898848947115328636715040578866337902750481566354238661203768010560056939935696678829394884407208311246423715319737062188883946712432742638151109800623047059726541476042502884419075341171231440736956555270413618581675255342293149119973622969239858152417678164812112068607"
typedef std:: string string;
void __FUNCTION__APATITE__print(const char* s)
{
	std::cout << s;
}
void __FUNCTION__APATITE__print(int s)
{
	std::cout << s;
}
void __FUNCTION__APATITE__print(float s)
{
	std::cout << s;
}
void __FUNCTION__APATITE__print(double s)
{
	std::cout << s;
}
void __FUNCTION__APATITE__print(char s)
{
	std::cout << s;
}
void __FUNCTION__APATITE__print(bool s)
{
	std::cout << s;
}
void __FUNCTION__APATITE__println(const char* s)
{
	std::cout << s << std::endl;
}
void __FUNCTION__APATITE__println(int s)
{
	std::cout << s << std::endl;
}
void __FUNCTION__APATITE__println(float s)
{
	std::cout << s << std::endl;
}
void __FUNCTION__APATITE__println(double s)
{
	std::cout << s << std::endl;
}
void __FUNCTION__APATITE__println(char s)
{
	std::cout << s << std::endl;
}
void __FUNCTION__APATITE__println(bool s)
{
	std::cout << s << std::endl;
}
int __FUNCTION__APATITE__inputInt()
{
	int a;
	std::cin>>a;
	return a;
}

string __FUNCTION__APATITE__inputString()
{
	string a;
	std::getline(std::cin,a);
	return a;
}
float __FUNCTION__APATITE__inputFloat()
{
	float a;
	std::cin>>a;
	return a;
}
double __FUNCTION__APATITE__inputDouble()
{
	double a;
	std::cin>>a;
	return a;
}
char __FUNCTION__APATITE__inputChar()
{
	char a;
	std::cin>>a;
	return a;
}
bool __FUNCTION__APATITE__inputBool()
{
	bool a;
	std::cin>>a;
	return a;
}
