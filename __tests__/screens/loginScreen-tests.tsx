import LoginScreen from '@/app/login';
import { render } from '@testing-library/react-native';

describe('<LoginScreen />', () => {
  test('Welocome text renders correctly on LoginScreen', () => {
    const { getByText } = render(<LoginScreen />);

    getByText('Welcome to your playground.');
  });
});
