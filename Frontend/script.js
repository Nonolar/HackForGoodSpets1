fetch("https://dindomän.se/api/code/1") // Byt till rätt URL
.then(response => response.json())
.then(data => {
  const codeElement = document.getElementById("code-display");
  codeElement.textContent = data.code;
  Prism.highlightElement(codeElement); // Syntax highlighting
});