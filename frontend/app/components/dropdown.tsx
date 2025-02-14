import React, { createRef, useEffect, useState } from 'react';
import {
  View,
  Text,
  TouchableOpacity,
  Animated,
  StyleSheet,
  LayoutAnimation,
  Platform,
  UIManager,
  Modal,
  Dimensions,
} from 'react-native';
import { ViewStyle, TextStyle } from 'react-native';

// Enable LayoutAnimation for Android
if (Platform.OS === 'android') {
  if (UIManager.setLayoutAnimationEnabledExperimental) {
    UIManager.setLayoutAnimationEnabledExperimental(true);
  }
}

const DropdownMenu = ({ 
  options, 
  defaultValue = 'Select an option',
  onSelect,
  containerStyle,
  dropdownStyle,
  optionStyle,
  textStyle,
}: DropdownProps) => {
  const [isOpen, setIsOpen] = useState(false);
  const [selectedOption, setSelectedOption] = useState(defaultValue);
  const [dropdownLayout, setDropdownLayout] = useState({ x: 0, y: 0, width: 0, height: 0 });
  const rotateAnimation = new Animated.Value(0);

  useEffect(() => {
    setSelectedOption(defaultValue);
  }, [defaultValue]);
  
  const toggleDropdown = () => {
    LayoutAnimation.configureNext(LayoutAnimation.Presets.easeInEaseOut);
    setIsOpen(!isOpen);
    Animated.timing(rotateAnimation, {
      toValue: isOpen ? 0 : 1,
      duration: 300,
      useNativeDriver: true,
    }).start();
  };

  const handleSelect = (option: string) => {
    setSelectedOption(option);
    setIsOpen(false);
    onSelect && onSelect(option);
    Animated.timing(rotateAnimation, {
      toValue: 0,
      duration: 300,
      useNativeDriver: true,
    }).start();
  };

  const rotateInterpolate = rotateAnimation.interpolate({
    inputRange: [0, 1],
    outputRange: ['0deg', '180deg'],
  });

  const animatedStyle = {
    transform: [{ rotate: rotateInterpolate }],
  };

  const measureDropdown = () => {
    if (dropdownRef?.current) {
      dropdownRef.current.measure((x, y, width, height, pageX, pageY) => {
        setDropdownLayout({
          x: pageX,
          y: pageY,
          width: width,
          height: height,
        });
      });
    }
  };

  const dropdownRef = createRef<View>()

  return (
    <View style={[styles.container, containerStyle]} ref={dropdownRef}>
      <TouchableOpacity 
        style={[styles.dropdownButton, dropdownStyle]} 
        onPress={() => {
          measureDropdown();
          toggleDropdown();
        }}
        activeOpacity={0.7}
      >
        <Text style={[styles.selectedText, textStyle]}>{selectedOption}</Text>
        <Animated.Text style={[styles.arrow, animatedStyle]}>▼</Animated.Text>
      </TouchableOpacity>
      
      <Modal
        visible={isOpen}
        transparent={true}
        animationType="none"
        onRequestClose={() => setIsOpen(false)}
      >
        <TouchableOpacity 
          style={styles.modalOverlay}
          activeOpacity={1}
          onPress={() => setIsOpen(false)}
        >
          <View 
            style={[
              styles.optionsContainer,
              dropdownStyle,
              {
                position: 'absolute',
                top: dropdownLayout.y + dropdownLayout.height + 4,
                left: dropdownLayout.x,
                width: dropdownLayout.width,
              }
            ]}
          >
            {options.map((option, index) => (
              <TouchableOpacity
                key={index}
                style={[styles.option, optionStyle]}
                onPress={() => handleSelect(option)}
                activeOpacity={0.7}
              >
                <Text style={[styles.optionText, textStyle]}>{option}</Text>
              </TouchableOpacity>
            ))}
          </View>
        </TouchableOpacity>
      </Modal>
    </View>
  );
};

interface DropdownProps {
  options: string[];
  defaultValue?: string;
  onSelect?: (selectedOption: string) => void;
  containerStyle?: ViewStyle;
  dropdownStyle?: ViewStyle;
  optionStyle?: ViewStyle;
  textStyle?: TextStyle;
}

const styles = StyleSheet.create({
  container: {
    width: '100%',
    maxWidth: 300,
  },
  dropdownButton: {
    flexDirection: 'row',
    justifyContent: 'space-between',
    alignItems: 'center',
    padding: 12,
    backgroundColor: '#fff',
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
  },
  selectedText: {
    fontSize: 16,
    color: '#333',
  },
  arrow: {
    fontSize: 12,
    color: '#666',
  },
  modalOverlay: {
    flex: 1,
    backgroundColor: 'transparent',
  },
  optionsContainer: {
    backgroundColor: '#fff',
    borderRadius: 8,
    borderWidth: 1,
    borderColor: '#ddd',
    shadowColor: '#000',
    shadowOffset: {
      width: 0,
      height: 2,
    },
    shadowOpacity: 0.25,
    shadowRadius: 3.84,
    elevation: 5,
  },
  option: {
    padding: 12,
    borderBottomWidth: 1,
    borderBottomColor: '#eee',
  },
  optionText: {
    fontSize: 16,
    color: '#333',
  },
});

export default DropdownMenu;