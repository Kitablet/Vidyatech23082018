//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace FISE {
    using System;
    using Xamarin.Forms;
    using Xamarin.Forms.Xaml;
    
    
    public partial class BookSearch : global::Xamarin.Forms.Grid {
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.StackLayout searchBtn;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.BoxView searchBtn_Box;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.Grid StackSearch;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::FISE.ViewModels.MyEntry searchText;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.BoxView searchByTextBtnBox;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.Grid searchByTextBtn;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.StackLayout BooksContainer;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.Grid NoBooksContainer;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.Image SearchItemsImage;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.StackLayout NoBooksTextContainer;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.Label NoBooksText1Container;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private global::Xamarin.Forms.Label NoBooksText2Container;
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("Xamarin.Forms.Build.Tasks.XamlG", "0.0.0.0")]
        private void InitializeComponent() {
            this.LoadFromXaml(typeof(BookSearch));
            searchBtn = this.FindByName<global::Xamarin.Forms.StackLayout>("searchBtn");
            searchBtn_Box = this.FindByName<global::Xamarin.Forms.BoxView>("searchBtn_Box");
            StackSearch = this.FindByName<global::Xamarin.Forms.Grid>("StackSearch");
            searchText = this.FindByName<global::FISE.ViewModels.MyEntry>("searchText");
            searchByTextBtnBox = this.FindByName<global::Xamarin.Forms.BoxView>("searchByTextBtnBox");
            searchByTextBtn = this.FindByName<global::Xamarin.Forms.Grid>("searchByTextBtn");
            BooksContainer = this.FindByName<global::Xamarin.Forms.StackLayout>("BooksContainer");
            NoBooksContainer = this.FindByName<global::Xamarin.Forms.Grid>("NoBooksContainer");
            SearchItemsImage = this.FindByName<global::Xamarin.Forms.Image>("SearchItemsImage");
            NoBooksTextContainer = this.FindByName<global::Xamarin.Forms.StackLayout>("NoBooksTextContainer");
            NoBooksText1Container = this.FindByName<global::Xamarin.Forms.Label>("NoBooksText1Container");
            NoBooksText2Container = this.FindByName<global::Xamarin.Forms.Label>("NoBooksText2Container");
        }
    }
}
