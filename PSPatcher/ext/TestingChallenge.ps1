# ------------------ Challenge Description ------------------

# 1. Построить AST для приведенного ниже скрипта (PowerShell со встроенным C#) (это можно сделать как встроенными средствами, так и сторонними) и произвести описанные ниже действия на его уровне;
# 2. Добавить новый публичный метод `Mul` для класса `Bar` в AST C#-скрипта, возвращающего произведение двух аргументов (int a, int b);
# 3. Заменить вызов метода `Add` на новосозданный `Mul` инстанса `BarInstance` в AST PowerShell-скрипта;
# 4. Произвести оптимизацию кода функции `foo` в AST PowerShell-скрипта (удаление мертвого кода);
# 5. Преобразовать результирующий AST обратно в PowerShell код со встроенным C#;
# 6. Написать юнит-тесты.
# 7. Нужна программная реализация всех описанных действий:
#    - Работа с AST;
#    - Добавление методов;
#    - Оптимизация кода.

# ------------------ Challenge Code ------------------

Set-Variable foo_arg -option ReadOnly -value 99

$BarCode = @"
public class Bar {
	public int Add(int a, int b) {
		return a + b;
	}
	
	public static float Divide(int a, int b) {
		return a / b;
	}
}
"@

Add-Type -TypeDefinition $BarCode
$BarInstance = New-Object -TypeName Bar

function foo($e) {
	$de = (!$e);

	if ($de) {
		$e = 43;
	}

	$a, $b = 20, ($e % 10);
	$c = $a + $b;

	if ($de) {
		$d = $c;
	} else {
		$d = $BarInstance.Add($c, $a);
	}

	if ($d -eq 100) {
		return -1;
	} elseif ($d -gt 100) {
		return $d;
	}

	return (Invoke-Expression $BarInstance.Add($b, $c));

}

$_foo = "foo($foo_arg)"

# Function "foo" return: 38
Write-Host 'Function "foo" return:' (Invoke-Expression $_foo)

# Function "foo" return: 580 (after modification)