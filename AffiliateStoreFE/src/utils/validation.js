

// validation email
export const ValidationEmail = (email) => {
  // Sử dụng regular expression để kiểm tra định dạng email
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};
