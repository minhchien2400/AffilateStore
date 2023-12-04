

// validation email
export const ValidationEmail = (email) => {
  // Sử dụng regular expression để kiểm tra định dạng email
  const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return emailRegex.test(email);
};

// validation password
export const ValidationPassword = (password) => {
  if (password.length < 8) {
    return false;
}

// Kiểm tra ít nhất một chữ cái viết hoa
if (!/[A-Z]/.test(password)) {
    return false;
}

// Kiểm tra ít nhất một chữ cái viết thường
if (!/[a-z]/.test(password)) {
    return false;
}

// Kiểm tra ít nhất một chữ số
if (!/\d/.test(password)) {
    return false;
}

// Kiểm tra ít nhất một ký tự đặc biệt
if (!/[!@#$%^&*]/.test(password)) {
    return false;
}

// Nếu đáp ứng tất cả các tiêu chí, mật khẩu là hợp lệ
return true;
}


// validate user name 
const validateUserName = (userName) => {
  // Kiểm tra độ dài tên người dùng
  if (userName.length < 5 || userName.length > 20) {
      return false;
  }

  // Kiểm tra ký tự đầu tiên và ký tự cuối cùng
  if (!/^[a-zA-Z0-9]/.test(userName) || !/[a-zA-Z0-9]$/.test(userName)) {
      return false;
  }

  // Kiểm tra xem tên người dùng chỉ chứa các ký tự hợp lệ
  if (!/^[a-zA-Z0-9_]+$/.test(userName)) {
      return false;
  }

  // Nếu đáp ứng tất cả các tiêu chí, tên người dùng là hợp lệ
  return true;
}