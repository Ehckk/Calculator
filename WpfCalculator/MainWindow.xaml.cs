using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace WpfCalculator {
	public partial class MainWindow : Window {
		private string sign;
		private double num1;

		private double Solve(string arg, double n1, double n2 = 0) {
			double answer;
			// remember to bind buttons for them as well 
			switch (arg) {
				case "-": // subtract
					answer = n1 - n2;
					break;
				case "*": // multiply
					answer = n1 * n2;
					break;
				case "/": // divide
					if (n2 == 0) {
						throw new DivideByZeroException(); // since input consists of doubles, C# does not throw DivByZero error, but since this is a calculator, I want it to
					} else {
						answer = n1 / n2;
					}
					break;
				case "%":
					answer = n2 * n1 / 100;
					break;
				case "^": // power
					answer = Math.Pow(n1, n2);
					break;
				case "sqrt": // sq root
					answer = Math.Sqrt(n1);
					break;
				case "root": // n2 root:
					answer = Math.Pow(n1, 1 / n2);
					break;
				case "mod":
					answer = n1 % n2;
					break;
				default: // addition or case "+":
					answer = n1 + n2;
					break;
			}
			return answer;
		}

		private static string SubText_Handler(string arg, double n1, double? n2 = null) {
			string subtxt;
			string a;
			if (arg != "sqrt") { // note that sqrt is absent since it only takes one number
				if (arg != "root") { // note that root is absent since it is formatted differently
					switch (arg) {
						case "mod":
							a = " %";
							break;
						case "%":
							a = "% of";
							break;
						default: // plus
							a = string.Format(" {0}", arg); // +, -, *, /, ^ buttons all use the same operator as their respective button content 
							break;
					}
					if (n2 == null) { // one number passed
						subtxt = string.Format("{0}{1}", n1, a);
					} else { // two numbers passed
						subtxt = string.Format("{0}{1} {2}", n1, a, n2);
					}
				} else { // arg == "root"
					if (n2 == null) { // one number passed
						subtxt = string.Format("root[ ]({0})", n1);
					} else { // two numbers passed
						subtxt = string.Format("root[{0}]({1})", n2, n1);
					}
				}
			} else { // arg == "sqrt"
				subtxt = string.Format("sqrt({0})", n1);
			}
			return subtxt;
		}

		public MainWindow() {
			InitializeComponent();
			Button_Clear_Click(Button_Back, new RoutedEventArgs());
		}

		private void Button_Number_Click(object sender, RoutedEventArgs e) {
			string b = (sender as Button).Content.ToString();
			if (b == "+/-") {
				if (TextBlock_Main.Text == string.Empty || TextBlock_Main.Text == "-") { // checks if there is a number input to flip, otherwise, adds/removes negative symbol
					if (TextBlock_Main.Text == string.Empty) {
						Input_Number("-"); // writes negative sign if user attempts to flip nothing 
					} else {
						Clear_TextBlock(TextBlock_Main); // clears tb_main if the user attempts to flip just a negative sign
					}
				} else { // the user uses the negative number button as intended
					double flip = Convert.ToDouble(TextBlock_Main.Text) * -1;
					TextBlock_Main.Text = flip.ToString();
				}
			} else {
				Input_Number(b);
			}

		}

		private void Button_Operator_Click(object sender, RoutedEventArgs e) { // make this another handler
			try {
				sign = (sender as Button).Content.ToString();
				num1 = Convert.ToDouble(TextBlock_Main.Text);
				TextBlock_Sub.Text = SubText_Handler(sign, num1);
				if (sign == "sqrt") {
					num1 = Solve(sign, num1);
					TextBlock_Main.Text = num1.ToString();
				} else {
					Clear_TextBlock(TextBlock_Main);
				}
			} catch (Exception error) {
				Error_Handler(error);
			}
 		}

		private void Button_Equals_Click(object sender, RoutedEventArgs e) {
			try {
				if (sign == null) {
					throw new FormatException();
				}
				double n2 = Convert.ToDouble(TextBlock_Main.Text);
				TextBlock_Sub.Text = SubText_Handler(sign, num1, n2);
				num1 = Solve(sign, num1, n2);
				TextBlock_Main.Text = num1.ToString(); // Puts answer in the input box
			} catch (Exception error) {
				Error_Handler(error);
			}
		}

		private void Button_Clear_Click(object sender, RoutedEventArgs e) {
			Clear_TextBlock(TextBlock_Sub);
			Clear_TextBlock(TextBlock_Main);
		}

		private void Button_Back_Click(object sender, RoutedEventArgs e) {
			if (TextBlock_Main.Text != string.Empty) {
				TextBlock_Main.Text = TextBlock_Main.Text.Remove(TextBlock_Main.Text.Length - 1, 1);
			} else {
				if (TextBlock_Sub.Text != string.Empty) {
					Clear_TextBlock(TextBlock_Sub);
				}
			}
		}

		private void Input_Number(string n) { // adds input to input box
			TextBlock_Main.Text = string.Format("{0}{1}", TextBlock_Main.Text, n);
		}

		private void KeyDown_Handler(object sender, KeyEventArgs e) { // add more shit here, even if you are just going fown the kwyboard
			bool shift;
			if (Keyboard.Modifiers == ModifierKeys.Shift) {
				shift = true;
			} else {
				shift = false;
			}
			switch (e.Key) {
				case Key.Escape:
					Button_Clear_Click(Button_Clear, new RoutedEventArgs());
					break;
				case Key.Back:
					Button_Back_Click(Button_Back, new RoutedEventArgs());
					break;
				case Key.S:
					Button_Operator_Click(Button_SqRt, new RoutedEventArgs());
					break;
				case Key.R:
					Button_Operator_Click(Button_AnyRt, new RoutedEventArgs());
					break;
				case Key.D7:
					Button_Number_Click(Button_7, new RoutedEventArgs());
					break;
				case Key.D8:
					if (shift) {
						Button_Operator_Click(Button_Multiply, new RoutedEventArgs());
					} else {
						Button_Number_Click(Button_8, new RoutedEventArgs());
					}
					break;
				case Key.D9:
					Button_Number_Click(Button_9, new RoutedEventArgs());
					break;
				case Key.OemPlus:
					Button_Operator_Click(Button_Add, new RoutedEventArgs());
					break;
				case Key.OemMinus:
					Button_Operator_Click(Button_Subtract, new RoutedEventArgs());
					break;
				case Key.D4:
					Button_Number_Click(Button_4, new RoutedEventArgs());
					break;
				case Key.D5:
					if (shift) {
						Button_Operator_Click(Button_Percent, new RoutedEventArgs());
					} else {
						Button_Number_Click(Button_5, new RoutedEventArgs());
					}
					break;
				case Key.D6:
					if (shift) {
						Button_Operator_Click(Button_Exponent, new RoutedEventArgs());
					} else {
						Button_Number_Click(Button_6, new RoutedEventArgs());
					}
					break;
				case Key.D1:
					Button_Number_Click(Button_1, new RoutedEventArgs());
					break;
				case Key.D2:
					Button_Number_Click(Button_2, new RoutedEventArgs());
					break;
				case Key.D3:
					Button_Number_Click(Button_3, new RoutedEventArgs());
					break;
				case Key.OemQuestion:
					Button_Operator_Click(Button_Divide, new RoutedEventArgs());
					break;
				case Key.M:
					Button_Operator_Click(Button_Modulo, new RoutedEventArgs());
					break;
				case Key.N:
					Button_Number_Click(Button_Negative, new RoutedEventArgs());
					break;
				case Key.D0:
					Button_Number_Click(Button_0, new RoutedEventArgs());
					break;
				case Key.OemPeriod:
					Button_Number_Click(Button_Decimal, new RoutedEventArgs());
					break;
				case Key.Enter:
					Button_Equals_Click(Button_Equals, new RoutedEventArgs());
					break;
				default:
					break;
			}
		}

		private void Clear_TextBlock(TextBlock tb) {
			tb.Text = string.Empty;
		}

		private void Error_Handler(Exception e) {
			string errorName = e.GetType().Name;
			string msgCaption = $"Fail - {errorName}";
			string msgContent;
			bool clearAll;
			MessageBoxImage msgImage;
			switch (errorName) {
				case "DivideByZeroException":
					msgContent = "You can't divide by zero bruh";
					msgImage = MessageBoxImage.Error;
					clearAll = false;
					break;
				case "FormatException":
					msgContent = "Invalid syntax";
					msgImage = MessageBoxImage.Warning;
					clearAll = true;
					break;
				default:
					msgContent = "I don't know what the fuck you did but you can't do that";
					msgImage = MessageBoxImage.Error;
					clearAll = true;
					break;
			}
			_ = MessageBox.Show(msgContent, msgCaption, MessageBoxButton.OK, msgImage, MessageBoxResult.OK);
			if (clearAll) {
				Button_Clear_Click(Button_Back, new RoutedEventArgs()); // basically hits the clear button
			} else {
				Clear_TextBlock(TextBlock_Main); // only clears input box
			}
		}

		private void Window_Loaded(object sender, RoutedEventArgs e) {
			Window window = GetWindow(this);
			window.KeyDown += KeyDown_Handler;
		}

		private void Button_Close_Click(object sender, RoutedEventArgs e) {
			Close();
		}

		private void Button_Minimize_Click(object sender, RoutedEventArgs e) {
			WindowState = WindowState.Minimized;
		}
	}
}
