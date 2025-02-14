import { StyleSheet } from 'react-native';

export const styles = StyleSheet.create({
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
    width: "90%"
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
    width: "100%"
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
},
addButton: {
    backgroundColor: '#00C853',
    padding: 8,
    borderRadius: 4,
    alignSelf: 'flex-start',
    marginTop: 16,
  },
  addButtonText: {
    color: '#fff',
    fontWeight: 'bold',
  },
  recipientsList: {
    paddingRight: 8,
  },
  chipContainer: {
    backgroundColor: '#e3f2fd',
    borderRadius: 20,
    flexDirection: 'row',
    alignItems: 'center',
    paddingHorizontal: 12,
    paddingVertical: 6,
    marginRight: 8,
    marginBottom: 8,
  },
  chipText: {
    color: '#1976d2',
    fontSize: 14,
    marginRight: 4,
  },
  removeButton: {
    padding: 2,
  },
  removeButtonText: {
    color: '#1976d2',
    fontSize: 18,
    fontWeight: 'bold',
  },
  suggestionsList: {
    maxHeight: 200,
    borderColor: '#ccc',
    borderWidth: 0,
    borderTopWidth: 0,
    borderBottomLeftRadius: 8,
    borderBottomRightRadius: 8,
  },
  suggestionItem: {
    padding: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  suggestionName: {
    fontSize: 16,
    color: 'white',
    marginBottom: 2,
  },
  suggestionEmail: {
    fontSize: 14,
    color: '#666',
  },

  // Org Page
  
});