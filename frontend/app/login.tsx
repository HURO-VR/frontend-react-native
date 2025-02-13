import { FBAuth } from '@/firebase/auth';
import { Redirect, useRouter } from 'expo-router';
import { useEffect, useState } from 'react';
import {
  View,
  Text,
  TextInput,
  TouchableOpacity,
  StyleSheet,
  SafeAreaView,
} from 'react-native';

const LoginScreen = () => {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [name, setName] = useState("")
  const [signUpToggle, setSignUpToggle] = useState(false)
  const [error, setError] = useState("")
  const [nameError, setNameError] = useState("")
  const [emailError, setEmailError] = useState("")
  const [passwordError, setPasswordError] = useState("")
  const [confirmPasswordError, setConfirmPasswordError] = useState("")
  const [confirmPassword, setConfirmPassword] = useState("")


  const [redirect, setRedirect] = useState("" as "/home")

  const handleLogin = () => {
    clearErrors()
    if (signUpToggle && ValidName() && passwordsMatch()) {
        FBAuth.createUser(email, password, name).then(() => {
            setRedirect("/home")
        }).catch((error) => handleError(error))
    } else if (!signUpToggle) {
        FBAuth.authenticate(email, password).then(() => {
          setRedirect("/home")
        }).catch((error) => handleError(error))
    }
    console.log('Login attempted with:', email, password);
  };

  const clearErrors = () => {
    setEmailError("")
    setPasswordError("")
    setNameError("")
    setError("")
    setConfirmPasswordError("")
  }

  const clearFields = () => {
    setPassword("")
    setEmail("")
    setName("")
  }

  const handleError = (error: string) => {
    if (error.includes("invalid-email")) {
      setEmailError("Invalid Email")
    } else if (error.includes("invalid-credential")) {
      setPasswordError("Invalid Password")
    } else if (error.includes("weak-password")) {
      setPasswordError("Please choose a stronger password.")
    } else if (error.includes("network-request-failed")) {
      setError("Failed to connect to server. Check internet connection.")
    } else {
      setError(error)
    } 
  }

  const ValidName = (): boolean => {
    let ret = true;
    if (name.length < 5) {
      setNameError("Name is too short.")
      ret = false
    } else if (!name.includes(" ")) {
      setNameError("Please include last name or initial.")
      ret = false
    }
    return ret
  }

  const passwordsMatch = () => {
    if (confirmPassword != password) {
      setConfirmPasswordError("Passwords do not match.")
      return false
    }
    return true
  }


  return (redirect.length == 0 ?
    <SafeAreaView style={styles.container}>
        <View style={{width: "60%"}}>

            <View style={styles.headerContainer}>
                <Text style={styles.title}>{signUpToggle ? "Sign Up" : "Login"}</Text>
            </View>

            <View style={styles.formContainer}>
                {signUpToggle && <View style={styles.inputContainer}>
                    <Text style={styles.label}>Name</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Enter your name"
                        placeholderTextColor="#666"
                        value={name}
                        onChangeText={setName}
                        autoCapitalize="words"
                    />
                    <Text style={styles.errorText}>{nameError}</Text>
                </View>}
                <View style={styles.inputContainer}>
                    <Text style={styles.label}>Email</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Enter your email"
                        placeholderTextColor="#666"
                        value={email}
                        onChangeText={setEmail}
                        keyboardType="email-address"
                        autoCapitalize="none"
                    />
                    <Text style={styles.errorText}>{emailError}</Text>
                </View>
                

                <View style={styles.inputContainer}>
                    <Text style={styles.label}>Password</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Enter your password"
                        placeholderTextColor="#666"
                        value={password}
                        onChangeText={setPassword}
                        onSubmitEditing={() => {
                          handleLogin()
                        }}
                        secureTextEntry
                    />
                    <Text style={styles.errorText}>{passwordError}</Text>
                </View>

                {signUpToggle && <View style={styles.inputContainer}>
                    <Text style={styles.label}>Confirm Password</Text>
                    <TextInput
                        style={styles.input}
                        placeholder="Confirm Password"
                        placeholderTextColor="#666"
                        onChangeText={setConfirmPassword}
                        value={confirmPassword}
                        onSubmitEditing={handleLogin}
                        secureTextEntry
                    />
                    <Text style={styles.errorText}>{confirmPasswordError}</Text>
                </View>}

                {error.length > 0 && <Text style={styles.errorText}>{error}</Text>}

                <TouchableOpacity 
                style={styles.loginButton}
                onPress={handleLogin}
                >
                <Text style={styles.loginButtonText}>{signUpToggle ? "Sign Up" : "Login"}</Text>
                </TouchableOpacity>

                <TouchableOpacity 
                style={styles.signUpToggle}
                onPress={() => {
                    clearErrors()
                    clearFields()
                    setSignUpToggle(!signUpToggle)
                }}
                >
                <Text style={styles.signUpText}>{!signUpToggle ? "Sign Up" : "Login"}</Text>
                </TouchableOpacity>
            </View>
      </View>
      {}
    </SafeAreaView>
  : <Redirect href={redirect}/>);
};

const styles = StyleSheet.create({
  container: {
    flex: 1,
    backgroundColor: '#000',
    padding: 20,
    alignItems: "center"
  },
  headerContainer: {
    marginTop: 60,
    marginBottom: 40,
  },
  logo: {
    color: '#fff',
    fontSize: 28,
    fontWeight: 'bold',
    marginBottom: 20,
  },
  title: {
    color: '#fff',
    fontSize: 24,
    fontWeight: '600',
  },
  formContainer: {
    flex: 1,
    justifyContent: 'flex-start',
  },
  inputContainer: {
    marginBottom: 20,
  },
  label: {
    color: '#fff',
    fontSize: 16,
    marginBottom: 8,
  },
  input: {
    backgroundColor: '#1a1a1a',
    borderRadius: 8,
    padding: 15,
    color: '#fff',
    fontSize: 16,
  },
  loginButton: {
    backgroundColor: '#00E676',
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    marginTop: 20,
    width: "50%",
    alignSelf: "center"
  },
  signUpToggle: {
    padding: 15,
    borderRadius: 8,
    alignItems: 'center',
    width: "30%",
    alignSelf: "center"
  },
  loginButtonText: {
    color: '#000',
    fontSize: 16,
    fontWeight: '600'
  },
  signUpText: {
    color: 'white',
    fontSize: 16,
    fontWeight: '200'
  },
  errorText: {
    color: '#ff3333',
    fontSize: 14,
    marginTop: 4,
    marginLeft: 2,
    fontWeight: '500',
}
});

export default LoginScreen;