import streamlit as st
import time
import keyboard
import os
import psutil

st.write("""
# My first app
Hello *world!*
""")

st.title("Demo")
# st.image(res, width = 800)

st.markdown("**Please fill the below form :**")
with st.form(key="Form :", clear_on_submit = True):
    File = st.file_uploader(label = "Upload file", type=["pdf","docx"])
    Submit = st.form_submit_button(label='Submit')
    

st.subheader("Details : ")
if Submit :
    st.markdown("**The file is sucessfully Uploaded.**")

        

exit_app = st.sidebar.button("Shut Down")
if exit_app:
    # Give a bit of delay for user experience
    time.sleep(5)
    # Close streamlit browser tab
    keyboard.press_and_release('ctrl+w')
    # Terminate streamlit python process
    pid = os.getpid()
    p = psutil.Process(pid)
    p.terminate()